import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductFormService } from '../services/product-form.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import {
  GetProductByIdQueryDto,
  UpdateProductCommand,
} from '../../../../../api-services/products/products-api.models';
import { ProductsApiService } from '../../../../../api-services/products/products-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { UserProfileService } from '../../../../../core/services/user-profile.service';

@Component({
  selector: 'app-products-edit',
  standalone: false,
  templateUrl: './products-edit.component.html',
  styleUrl: './products-edit.component.scss',
  providers: [ProductFormService],
})
export class ProductsEditComponent
  extends BaseFormComponent<GetProductByIdQueryDto>
  implements OnInit
{
  private api = inject(ProductsApiService);
  private formService = inject(ProductFormService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private profileService = inject(UserProfileService);

  productId!: number;

  ngOnInit(): void {
    this.productId = +this.route.snapshot.params['id'];
    this.initForm(true);
  }

  protected loadData(): void {
    this.startLoading();
    this.api.getById(this.productId).subscribe({
      next: (product) => {
        this.model = product;
        this.form = this.formService.createProductForm(product);
        this.stopLoading();
      },
      error: () => {
        this.stopLoading('Failed to load product');
        this.toaster.error('Product not found');
        this.router.navigate(['/admin/products']);
      },
    });
  }

  protected save(): void {
    if (this.form.invalid || this.isLoading) return;

    const variantError = this.formService.validateVariants(this.form);
    if (variantError) {
      this.toaster.error(variantError);
      return;
    }

    const gymId = this.model?.gymId ?? this.profileService.profile()?.gymId;
    if (!gymId) {
      this.toaster.error('No gym on profile');
      return;
    }

    this.startLoading();
    const v = this.form.getRawValue();
    const variants = this.formService.mapVariants(this.form);
    const totals = this.formService.aggregateFromVariants(variants);

    const command: UpdateProductCommand = {
      name: v.name,
      categoryName: v.categoryName,
      description: v.description || null,
      price: totals.price,
      stockQuantity: totals.stockQuantity,
      gymId,
      variants,
    };

    this.api.update(this.productId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Product updated');
        this.router.navigate(['/admin/products']);
      },
      error: (err) => {
        this.stopLoading('Failed to update product');
        const msg =
          err?.error?.detail ?? err?.error?.message ?? 'Could not update product';
        this.toaster.error(msg);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/products']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
