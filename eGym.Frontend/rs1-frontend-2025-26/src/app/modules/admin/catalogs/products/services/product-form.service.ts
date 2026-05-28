import { Injectable, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  GetProductByIdQueryDto,
  ProductVariantCommandDto,
  ProductVariantQueryDto,
} from '../../../../../api-services/products/products-api.models';
import {
  hasDuplicateCanonicalVariants,
  normalizeSizeForStorage,
} from '../../../../catalog/products/product-variant.utils';

interface VariantRowValue {
  id: number;
  size: string;
  color: string;
  price: number;
  stockQuantity: number;
}

@Injectable()
export class ProductFormService {
  private fb = inject(FormBuilder);

  createProductForm(product?: GetProductByIdQueryDto): FormGroup {
    const form = this.fb.group({
      name: [
        product?.name ?? '',
        [Validators.required, Validators.minLength(3), Validators.maxLength(150)],
      ],
      categoryName: [
        product?.categoryName ?? '',
        [Validators.required, Validators.maxLength(100)],
      ],
      description: [product?.description ?? '', [Validators.maxLength(500)]],
      variants: this.fb.array<FormGroup>([]),
    });

    const variantsArray = this.variantsArray(form);
    const source = product?.productVariants ?? [];

    if (source.length) {
      source.forEach((variant) => variantsArray.push(this.createVariantGroup(variant)));
    } else if (product) {
      variantsArray.push(
        this.createVariantGroup({
          id: 0,
          productId: product.id,
          size: 'Standard',
          color: 'Standard',
          price: product.price,
          stockQuantity: product.stockQuantity,
        }),
      );
    } else {
      variantsArray.push(this.createVariantGroup());
    }

    return form;
  }

  variantsArray(form: FormGroup): FormArray<FormGroup> {
    return form.get('variants') as FormArray<FormGroup>;
  }

  createVariantGroup(variant?: ProductVariantQueryDto): FormGroup {
    return this.fb.group({
      id: [variant?.id ?? 0],
      size: [variant?.size ?? '', [Validators.required, Validators.maxLength(50)]],
      color: [variant?.color ?? '', [Validators.required, Validators.maxLength(100)]],
      price: [
        variant?.price ?? 0,
        [Validators.required, Validators.min(0.01), Validators.max(1000000)],
      ],
      stockQuantity: [
        variant?.stockQuantity ?? 0,
        [Validators.required, Validators.min(0)],
      ],
    });
  }

  mapVariants(form: FormGroup): ProductVariantCommandDto[] {
    return this.variantsArray(form).controls.map((group) => {
      const row = group.getRawValue() as VariantRowValue;
      return {
        id: row['id'] > 0 ? row['id'] : null,
        size: normalizeSizeForStorage(String(row['size'] ?? '')),
        color: String(row['color'] ?? '').trim(),
        price: Number(row['price']),
        stockQuantity: Number(row['stockQuantity']),
      };
    });
  }

  validateVariants(form: FormGroup): string | null {
    const variants = this.mapVariants(form);
    if (!variants.length) {
      return 'At least one variant is required.';
    }
    if (hasDuplicateCanonicalVariants(variants)) {
      return 'Duplicate size and flavor. Values like 450 and 450g are treated as the same size.';
    }
    return null;
  }

  aggregateFromVariants(variants: ProductVariantCommandDto[]): { price: number; stockQuantity: number } {
    if (!variants.length) return { price: 0, stockQuantity: 0 };
    return {
      price: Math.min(...variants.map((v) => v.price)),
      stockQuantity: variants.reduce((sum, v) => sum + v.stockQuantity, 0),
    };
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;
    if (errors['required']) return 'This field is required';
    if (errors['minlength'])
      return `Minimum ${errors['minlength'].requiredLength} characters required`;
    if (errors['maxlength'])
      return `Maximum ${errors['maxlength'].requiredLength} characters allowed`;
    if (errors['min']) return `Minimum value is ${errors['min'].min}`;
    if (errors['max']) return `Maximum value is ${errors['max'].max}`;
    return 'Invalid value';
  }

  getVariantErrorMessage(variant: FormGroup, controlName: string): string {
    return this.getErrorMessage(variant, controlName);
  }
}
