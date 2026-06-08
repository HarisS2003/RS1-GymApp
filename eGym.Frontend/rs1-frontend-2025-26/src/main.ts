import { platformBrowser } from '@angular/platform-browser';
import * as Sentry from '@sentry/angular';
import { AppModule } from './app/app-module';

import 'zone.js';

Sentry.init({
  dsn: 'https://f5fc038b2c959f6c51a88480ab2859d8@o4511520978763776.ingest.de.sentry.io/4511520995278928',
  sendDefaultPii: true,
});

platformBrowser()
  .bootstrapModule(AppModule)
  .catch((err) => console.error(err));
