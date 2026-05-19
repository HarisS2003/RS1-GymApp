import { NgModule } from '@angular/core';

import { PublicRoutingModule } from './public-routing-module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SearchProductsComponent } from './search-products/search-products.component';
import { GymCardComponent } from './gym-card/gym-card.component';
import { SharedModule } from '../shared/shared-module';

@NgModule({
  declarations: [PublicLayoutComponent, SearchProductsComponent, GymCardComponent],
  imports: [SharedModule, PublicRoutingModule],
})
export class PublicModule {}
