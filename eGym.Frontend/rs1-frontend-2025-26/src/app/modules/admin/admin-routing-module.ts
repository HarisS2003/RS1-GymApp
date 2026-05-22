import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './catalogs/products/products.component';
import { ProductsAddComponent } from './catalogs/products/products-add/products-add.component';
import { ProductsEditComponent } from './catalogs/products/products-edit/products-edit.component';
import { ProductCategoriesComponent } from './catalogs/product-categories/product-categories.component';
import {AdminOrdersComponent} from './orders/admin-orders.component';
import {AdminSettingsComponent} from './admin-settings/admin-settings.component';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard.component';
import { MembershipPlansComponent } from './catalogs/membership-plans/membership-plans.component';
import { MembershipPlansAddComponent } from './catalogs/membership-plans/membership-plans-add/membership-plans-add.component';
import { MembershipPlansEditComponent } from './catalogs/membership-plans/membership-plans-edit/membership-plans-edit.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: 'dashboard', component: AdminDashboardComponent },

      // PRODUCTS
      {
        path: 'products',
        component: ProductsComponent,
      },
      {
        path: 'products/add',
        component: ProductsAddComponent,
      },
      {
        path: 'products/:id/edit',
        component: ProductsEditComponent,
      },

      // MEMBERSHIP PLANS
      {
        path: 'membership-plans',
        component: MembershipPlansComponent,
      },
      {
        path: 'membership-plans/add',
        component: MembershipPlansAddComponent,
      },
      {
        path: 'membership-plans/:id/edit',
        component: MembershipPlansEditComponent,
      },

      // PRODUCT CATEGORIES
      {
        path: 'product-categories',
        component: ProductCategoriesComponent,
      },

      {
        path: 'orders',
        component: AdminOrdersComponent,
      },

      {
        path: 'settings',
        component: AdminSettingsComponent,
      },


      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
