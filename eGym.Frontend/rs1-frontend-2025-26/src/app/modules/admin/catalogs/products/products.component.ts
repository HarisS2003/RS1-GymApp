// products.component.ts

import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  ListProductsRequest,
  ListProductsQueryDto
} from '../../../../api-services/products/products-api.models';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent
  extends BaseListPagedComponent<ListProductsQueryDto, ListProductsRequest>
  implements OnInit {

  private api = inject(ProductsApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);

  totalProducts = 0;
  activeProducts = 0;
  lowStockCount = 0;

  constructor() {
    super();
    this.request = new ListProductsRequest();
  }

  ngOnInit(): void {
    this.initList();
    this.loadProductStats();
  }

  goTrainers(): void {
    this.router.navigate(['/admin/dashboard']);
  }

  goMemberships(): void {
    this.router.navigate(['/admin/membership-plans']);
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: () => {
        this.stopLoading('Failed to load products');
      },
    });
  }

  private loadProductStats(): void {
    const statsReq = new ListProductsRequest();
    statsReq.paging.page = 1;
    statsReq.paging.pageSize = 500;

    this.api.list(statsReq).subscribe({
      next: (res) => {
        const all = res.items ?? [];
        this.totalProducts = res.totalItems ?? all.length;
        this.activeProducts = all.length;
        this.lowStockCount = all.filter((p) => p.stockQuantity < 5).length;
      },
    });
  }

  // === UI Actions ===

  onCreate(): void {
    this.router.navigate(['/admin/products/add']);
  }

  onEdit(product: ListProductsQueryDto): void {
    this.router.navigate(['/admin/products', product.id, 'edit']);
  }

  onDelete(product: ListProductsQueryDto): void {
    this.dialogHelper.product.confirmDelete(product.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(product);
      }
    });
  }

  private performDelete(product: ListProductsQueryDto): void {
    this.startLoading();

    this.api.delete(product.id).subscribe({
      next: () => {
        this.dialogHelper.product.showDeleteSuccess().subscribe();
        this.loadPagedData();
        this.loadProductStats();
      },
      error: (err) => {
        this.stopLoading();

        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'PRODUCTS.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete product error:', err);
      }
    });
  }

  onSearch(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}
