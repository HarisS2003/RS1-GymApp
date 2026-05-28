import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { withPageAnimation } from '../../core/animations/route-animations';

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
import { AdminShopComponent } from './shop/admin-shop.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: 'dashboard', component: AdminDashboardComponent, data: withPageAnimation('AdminDashboard') },

      // PRODUCTS
      {
        path: 'products',
        component: ProductsComponent,
        data: withPageAnimation('AdminProducts'),
      },
      {
        path: 'products/add',
        component: ProductsAddComponent,
        data: withPageAnimation('AdminProductsAdd'),
      },
      {
        path: 'products/:id/edit',
        component: ProductsEditComponent,
        data: withPageAnimation('AdminProductsEdit'),
      },

      // MEMBERSHIP PLANS
      {
        path: 'membership-plans',
        component: MembershipPlansComponent,
        data: withPageAnimation('AdminMembershipPlans'),
      },
      {
        path: 'membership-plans/add',
        component: MembershipPlansAddComponent,
        data: withPageAnimation('AdminMembershipPlansAdd'),
      },
      {
        path: 'membership-plans/:id/edit',
        component: MembershipPlansEditComponent,
        data: withPageAnimation('AdminMembershipPlansEdit'),
      },

      // PRODUCT CATEGORIES
      {
        path: 'product-categories',
        component: ProductCategoriesComponent,
        data: withPageAnimation('AdminProductCategories'),
      },

      {
        path: 'orders',
        component: AdminOrdersComponent,
        data: withPageAnimation('AdminOrders'),
      },

      {
        path: 'shop',
        component: AdminShopComponent,
        data: withPageAnimation('AdminShop'),
      },

      {
        path: 'settings',
        component: AdminSettingsComponent,
        data: withPageAnimation('AdminSettings'),
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
