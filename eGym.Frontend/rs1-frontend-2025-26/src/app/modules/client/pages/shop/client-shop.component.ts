import { Component, OnInit, inject } from '@angular/core';
import { catchError, of } from 'rxjs';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ListProductsRequest } from '../../../../api-services/products/products-api.models';
import { ListProductsQueryDto } from '../../../../api-services/products/products-api.models';

@Component({
  selector: 'app-client-shop',
  standalone: false,
  templateUrl: './client-shop.component.html',
  styleUrl: './client-shop.component.scss',
})
export class ClientShopComponent implements OnInit {
  private productsApi = inject(ProductsApiService);

  loading = true;
  products: ListProductsQueryDto[] = [];

  ngOnInit(): void {
    const req = new ListProductsRequest();
    req.paging.pageSize = 100;

    this.productsApi
      .list(req)
      .pipe(catchError(() => of({ items: [] } as any)))
      .subscribe({
        next: (res) => {
          this.products = (res.items ?? []).filter(
            (p: ListProductsQueryDto) => p.isEnabled,
          );
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
