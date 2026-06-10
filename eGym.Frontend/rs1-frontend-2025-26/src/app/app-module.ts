import { ErrorHandler, NgModule, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideAnimations} from '@angular/platform-browser/animations';
import {HttpClient, provideHttpClient, withFetch, withInterceptors} from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { AppComponent } from './app.component';
import {authInterceptor} from './core/interceptors/auth-interceptor.service';
import {loadingBarInterceptor} from './core/interceptors/loading-bar-interceptor.service';
import {errorLoggingInterceptor} from './core/interceptors/error-logging-interceptor.service';
import {serverApiBaseUrlInterceptor} from './core/interceptors/server-api-base-url-interceptor.service';
import {transferStateInterceptor} from './core/interceptors/transfer-state-interceptor.service';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {CustomTranslateLoader} from './core/services/custom-translate-loader';
import {materialModules} from './modules/shared/material-modules';
import {SharedModule} from './modules/shared/shared-module';
import { SentryErrorHandler } from './core/sentry-error-handler';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (http: HttpClient) => new CustomTranslateLoader(http),
        deps: [HttpClient]
      }
    }),
    SharedModule,
    materialModules,
  ],
  providers: [
    { provide: ErrorHandler, useClass: SentryErrorHandler },
    provideAnimations(),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection(),
    provideHttpClient(
      withFetch(),
      withInterceptors([
        transferStateInterceptor,
        serverApiBaseUrlInterceptor,
        loadingBarInterceptor,
        authInterceptor,
        errorLoggingInterceptor
      ])
    ),
    provideClientHydration(withEventReplay())
  ],
  exports: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
