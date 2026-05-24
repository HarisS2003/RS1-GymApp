import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FitPaginatorBarComponent} from './components/fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './material-modules';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import { FitConfirmDialogComponent } from './components/fit-confirm-dialog/fit-confirm-dialog.component';
import {DialogHelperService} from './services/dialog-helper.service';
import { FitLoadingBarComponent } from './components/fit-loading-bar/fit-loading-bar.component';
import { FitTableSkeletonComponent } from './components/fit-table-skeleton/fit-table-skeleton.component';
import { LocaleThemeToolbarComponent } from './components/locale-theme-toolbar/locale-theme-toolbar.component';
import { AccountProfileComponent } from './components/account-profile/account-profile.component';
import { MembershipPurchaseDialogComponent } from './components/membership-purchase-dialog/membership-purchase-dialog.component';
import { MembershipAlreadyHaveDialogComponent } from './components/membership-already-have-dialog/membership-already-have-dialog.component';

@NgModule({
  declarations: [
    FitPaginatorBarComponent,
    FitConfirmDialogComponent,
    MembershipPurchaseDialogComponent,
    MembershipAlreadyHaveDialogComponent,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LocaleThemeToolbarComponent,
    AccountProfileComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    ...materialModules
  ],
  providers: [
    DialogHelperService
  ],
  exports:[
    FitPaginatorBarComponent,
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LocaleThemeToolbarComponent,
    AccountProfileComponent,
    materialModules,
  ],
})
export class SharedModule { }
