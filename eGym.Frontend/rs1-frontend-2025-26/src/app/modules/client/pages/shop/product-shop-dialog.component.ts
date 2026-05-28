import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  GetProductByIdQueryDto,
  ProductVariantQueryDto,
} from '../../../../api-services/products/products-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';
import { TranslateService } from '@ngx-translate/core';
import {
  canonicalSizeKey,
  displaySizeLabel,
  findVariantBySelection,
  uniqueFlavorLabels,
} from '../../../catalog/products/product-variant.utils';

export interface SizeOption {
  key: string;
  label: string;
  hasStock: boolean;
}

export interface FlavorOption {
  value: string;
  availableForSize: boolean;
  stockForSize: number;
}

@Component({
  selector: 'app-product-shop-dialog',
  standalone: false,
  templateUrl: './product-shop-dialog.component.html',
  styleUrl: './product-shop-dialog.component.scss',
})
export class ProductShopDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<ProductShopDialogComponent>);
  product = inject<GetProductByIdQueryDto>(MAT_DIALOG_DATA);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  private variants: ProductVariantQueryDto[] = [];

  /** Canonical size key (e.g. 450g), not raw DB string. */
  selectedSizeKey: string | null = null;
  selectedFlavor: string | null = null;

  ngOnInit(): void {
    this.variants = (this.product.productVariants ?? []).filter(
      (v) => v.size?.trim() && v.color?.trim(),
    );

    const firstInStockSize = this.sizeOptions.find((s) => s.hasStock)?.key ?? null;
    this.selectedSizeKey = firstInStockSize;
    this.syncFlavorAfterSizeChange();
  }

  get isSupplement(): boolean {
    const category = (this.product.categoryName ?? '').toLowerCase();
    return (
      category.includes('supplement') ||
      category.includes('suplement') ||
      category.includes('supplements')
    );
  }

  get flavorLabelKey(): string {
    return this.isSupplement ? 'CLIENT.SHOP.FLAVOR' : 'CLIENT.SHOP.COLOR';
  }

  get sizeOptions(): SizeOption[] {
    const keys = [
      ...new Set(this.variants.map((v) => canonicalSizeKey(v.size)).filter(Boolean)),
    ];

    return keys
      .map((key) => ({
        key,
        label: displaySizeLabel(key, this.variants),
        hasStock: this.variants.some(
          (v) => canonicalSizeKey(v.size) === key && v.stockQuantity > 0,
        ),
      }))
      .sort((a, b) => a.label.localeCompare(b.label, undefined, { numeric: true }));
  }

  get allFlavorValues(): string[] {
    return uniqueFlavorLabels(this.variants);
  }

  get flavorOptions(): FlavorOption[] {
    return this.allFlavorValues.map((flavor) => {
      const match = this.selectedSizeKey
        ? findVariantBySelection(this.variants, this.selectedSizeKey, flavor)
        : null;

      return {
        value: flavor,
        availableForSize: !!match && match.stockQuantity > 0,
        stockForSize: match?.stockQuantity ?? 0,
      };
    });
  }

  get selectedVariant(): ProductVariantQueryDto | null {
    if (!this.selectedSizeKey || !this.selectedFlavor) return null;
    return findVariantBySelection(this.variants, this.selectedSizeKey, this.selectedFlavor);
  }

  get selectionComplete(): boolean {
    return this.selectedVariant !== null;
  }

  get displayPrice(): number | null {
    return this.selectedVariant?.price ?? null;
  }

  get displayStock(): number | null {
    return this.selectedVariant?.stockQuantity ?? null;
  }

  get outOfStock(): boolean {
    if (!this.selectionComplete) return false;
    return (this.displayStock ?? 0) <= 0;
  }

  get showFlavorGroup(): boolean {
    return this.allFlavorValues.length > 0;
  }

  get showSizeGroup(): boolean {
    return this.sizeOptions.length > 0;
  }

  onSizeSelect(sizeKey: string): void {
    const option = this.sizeOptions.find((s) => s.key === sizeKey);
    if (!option?.hasStock) return;

    this.selectedSizeKey = sizeKey;
    this.syncFlavorAfterSizeChange();
  }

  onFlavorSelect(flavor: string): void {
    const option = this.flavorOptions.find((f) => f.value === flavor);
    if (!this.selectedSizeKey || !option?.availableForSize) return;

    this.selectedFlavor = flavor;
  }

  isSizeDisabled(size: SizeOption): boolean {
    return !size.hasStock;
  }

  isFlavorDisabled(option: FlavorOption): boolean {
    return !this.selectedSizeKey || !option.availableForSize;
  }

  addToBasket(): void {
    if (!this.selectionComplete || this.outOfStock) return;
    this.toaster.info(this.translate.instant('CLIENT.SHOP.BASKET_NOT_READY'));
  }

  close(): void {
    this.dialogRef.close();
  }

  private syncFlavorAfterSizeChange(): void {
    if (!this.selectedSizeKey) {
      this.selectedFlavor = null;
      return;
    }

    const stillValid = this.selectedFlavor
      ? this.flavorOptions.find(
          (f) => f.value === this.selectedFlavor && f.availableForSize,
        )
      : null;

    if (stillValid) return;

    this.selectedFlavor =
      this.flavorOptions.find((f) => f.availableForSize)?.value ?? null;
  }
}
