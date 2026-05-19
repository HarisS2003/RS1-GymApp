import { Component, OnInit, inject } from '@angular/core';
import { AppLocaleService } from './core/services/app-locale.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private locale = inject(AppLocaleService);
  private theme = inject(ThemeService);

  ngOnInit(): void {
    this.locale.init();
    this.theme.init();
  }
}
