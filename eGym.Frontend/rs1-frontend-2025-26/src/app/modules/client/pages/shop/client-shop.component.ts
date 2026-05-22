import { Component, OnInit, inject } from '@angular/core';
import { catchError, of } from 'rxjs';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ListProductsRequest } from '../../../../api-services/products/products-api.models';
import { ListProductsQueryDto } from '../../../../api-services/products/products-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';

@Component({
  selector: 'app-client-shop',
  standalone: false,
  templateUrl: './client-shop.component.html',
  styleUrl: './client-shop.component.scss',
})
export class ClientShopComponent implements OnInit {
  private productsApi = inject(ProductsApiService);
  private profileService = inject(UserProfileService);

  loading = true;
  products: ListProductsQueryDto[] = [];

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
}
