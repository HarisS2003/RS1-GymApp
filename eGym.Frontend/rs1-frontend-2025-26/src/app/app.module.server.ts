import { NgModule } from '@angular/core';
import { provideServerRendering, withRoutes } from '@angular/ssr';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { AppModule } from './app-module';
import { serverRoutes } from './app.routes.server';

@NgModule({
  imports: [AppModule],
  providers: [
    provideServerRendering(withRoutes(serverRoutes)),
    // Server ignores animation processing — overrides AppModule's provideAnimations on SSR.
    provideNoopAnimations(),
  ],
  bootstrap: [AppComponent],
})
export class AppServerModule {}
