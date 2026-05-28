import { Component, Input, inject } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';
import { ProductFormService } from '../services/product-form.service';

@Component({
  selector: 'app-product-variants-editor',
  standalone: false,
  templateUrl: './product-variants-editor.component.html',
  styleUrl: './product-variants-editor.component.scss',
})
export class ProductVariantsEditorComponent {
  @Input({ required: true }) form!: FormGroup;

  private formService = inject(ProductFormService);

  get variants(): FormArray<FormGroup> {
    return this.formService.variantsArray(this.form);
  }

  addVariant(): void {
    this.variants.push(this.formService.createVariantGroup());
  }

  removeVariant(index: number): void {
    if (this.variants.length <= 1) return;
    this.variants.removeAt(index);
  }

  variantError(variant: FormGroup, controlName: string): string {
    return this.formService.getVariantErrorMessage(variant, controlName);
  }

  hasVariantError(variant: FormGroup, controlName: string): boolean {
    const control = variant.get(controlName);
    return !!control && control.invalid && control.touched;
  }
}
