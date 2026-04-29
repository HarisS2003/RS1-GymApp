import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-crud-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './crud-page.component.html'
})
export class CrudPageComponent {
  private readonly route = inject(ActivatedRoute);
  readonly resource = computed(() => this.route.snapshot.data['resource'] as string ?? 'resource');
}
