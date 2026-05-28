import { Component, OnInit, inject } from '@angular/core';
import { MatChipListboxChange } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { catchError, of } from 'rxjs';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ListProductsRequest } from '../../../../api-services/products/products-api.models';
import { ListProductsQueryDto } from '../../../../api-services/products/products-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ProductShopDialogComponent } from './product-shop-dialog.component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { TranslateService } from '@ngx-translate/core';
import { productGridAnimation } from '../../../../core/animations/ui.animations';

@Component({
  selector: 'app-client-shop',
  standalone: false,
  templateUrl: './client-shop.component.html',
  styleUrl: './client-shop.component.scss',
  animations: [productGridAnimation],
})
export class ClientShopComponent implements OnInit {
  private productsApi = inject(ProductsApiService);
  private profileService = inject(UserProfileService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  loading = true;
  detailLoading = false;
  products: ListProductsQueryDto[] = [];
  categories: string[] = [];
  selectedCategory: string | null = null;

  ngOnInit(): void {
    const load = () => {
      const req = new ListProductsRequest();
      req.paging.pageSize = 500;
      const gymId = this.profileService.profile()?.gymId;
      if (gymId) req.gymId = gymId;

      this.productsApi
        .list(req)
        .pipe(catchError(() => of({ items: [] } as any)))
        .subscribe({
          next: (res) => {
            this.products = res.items ?? [];
            this.categories = [
              ...new Set(this.products.map((p) => p.categoryName).filter(Boolean)),
            ].sort((a, b) => a.localeCompare(b));
            this.loading = false;
          },
          error: () => (this.loading = false),
        });
    };

    if (this.profileService.profile()) {
      load();
    } else {
      this.profileService.loadProfile().subscribe(() => load());
    }
  }

  get filteredProducts(): ListProductsQueryDto[] {
    if (!this.selectedCategory) return this.products;
    return this.products.filter((p) => p.categoryName === this.selectedCategory);
  }

  /** Changes when filters change — drives stagger animation on the grid. */
  get productGridKey(): string {
    return `${this.selectedCategory ?? 'all'}:${this.filteredProducts.length}`;
  }

  selectCategory(category: string | null): void {
    this.selectedCategory = category;
  }

  onCategoryChipChange(event: MatChipListboxChange): void {
    const value = event.value as string;
    this.selectCategory(value ? value : null);
  }

  openProduct(product: ListProductsQueryDto): void {
    if (this.detailLoading) return;

    this.detailLoading = true;
    this.productsApi.getById(product.id).subscribe({
      next: (detail) => {
        this.detailLoading = false;
        this.dialog.open(ProductShopDialogComponent, {
          data: detail,
          width: '520px',
          maxWidth: '95vw',
          panelClass: 'product-shop-dialog-panel',
          enterAnimationDuration: 0,
          exitAnimationDuration: 0,
        });
      },
      error: () => {
        this.detailLoading = false;
        this.toaster.error(this.translate.instant('CLIENT.SHOP.LOAD_ERROR'));
      },
    });
  }

  displayPrice(product: ListProductsQueryDto): number {
    return product.price;
  }
}
