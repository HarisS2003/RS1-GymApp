import { Component, OnInit, inject } from '@angular/core';
import { catchError, of } from 'rxjs';
import {
  ListProductsQueryDto,
  ListProductsRequest,
} from '../../../api-services/products/products-api.models';
import { ProductsApiService } from '../../../api-services/products/products-api.service';
import { UserProfileService } from '../../../core/services/user-profile.service';

@Component({
  selector: 'app-admin-shop',
  standalone: false,
  templateUrl: './admin-shop.component.html',
  styleUrl: './admin-shop.component.scss',
})
export class AdminShopComponent implements OnInit {
  private productsApi = inject(ProductsApiService);
  private profileService = inject(UserProfileService);

  loading = true;
  products: ListProductsQueryDto[] = [];

  ngOnInit(): void {
    if (this.profileService.profile()) {
      this.loadProducts();
      return;
    }
    this.profileService.loadProfile().subscribe(() => this.loadProducts());
  }

  private loadProducts(): void {
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
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
