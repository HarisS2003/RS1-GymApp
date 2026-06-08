import {NgModule} from '@angular/core';

import {AdminRoutingModule} from './admin-routing-module';
import {ProductsComponent} from './catalogs/products/products.component';
import {ProductsAddComponent} from './catalogs/products/products-add/products-add.component';
import {ProductsEditComponent} from './catalogs/products/products-edit/products-edit.component';
import {AdminLayoutComponent} from './admin-layout/admin-layout.component';
import {ProductCategoriesComponent} from './catalogs/product-categories/product-categories.component';
import {
  ProductCategoryUpsertComponent
} from './catalogs/product-categories/product-category-upsert/product-category-upsert.component';
import {AdminOrdersComponent} from './orders/admin-orders.component';
import {AdminSettingsComponent} from './admin-settings/admin-settings.component';
import {SharedModule} from '../shared/shared-module';
import { OrderDetailsDialogComponent } from './orders/admin-orders-details-dialog/order-details-dialog.component';
import { ChangeStatusDialogComponent } from './orders/change-status-dialog/change-status-dialog.component';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard.component';
import { AdminUsersComponent } from './pages/admin-users/admin-users.component';
import { AddTrainerDialogComponent } from './pages/add-trainer-dialog/add-trainer-dialog.component';
import { EditTrainerDialogComponent } from './pages/edit-trainer-dialog/edit-trainer-dialog.component';
import { EditUserDialogComponent } from './pages/edit-user-dialog/edit-user-dialog.component';
import { MembershipHistoryDialogComponent } from './pages/membership-history-dialog/membership-history-dialog.component';
import { MembershipPlansComponent } from './catalogs/membership-plans/membership-plans.component';
import { MembershipPlansAddComponent } from './catalogs/membership-plans/membership-plans-add/membership-plans-add.component';
import { MembershipPlansEditComponent } from './catalogs/membership-plans/membership-plans-edit/membership-plans-edit.component';
import { AdminShopComponent } from './shop/admin-shop.component';
import { ProductVariantsEditorComponent } from './catalogs/products/product-variants-editor/product-variants-editor.component';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    AdminUsersComponent,
    AddTrainerDialogComponent,
    EditTrainerDialogComponent,
    EditUserDialogComponent,
    MembershipHistoryDialogComponent,
    ProductsComponent,
    ProductsAddComponent,
    ProductsEditComponent,
    ProductVariantsEditorComponent,
    MembershipPlansComponent,
    MembershipPlansAddComponent,
    MembershipPlansEditComponent,
    AdminLayoutComponent,
    ProductCategoriesComponent,
    ProductCategoryUpsertComponent,
    AdminOrdersComponent,
    AdminSettingsComponent,
    OrderDetailsDialogComponent,
    ChangeStatusDialogComponent,
    AdminShopComponent,
  ],
  imports: [
    AdminRoutingModule,
    SharedModule,
  ]
})
export class AdminModule { }
