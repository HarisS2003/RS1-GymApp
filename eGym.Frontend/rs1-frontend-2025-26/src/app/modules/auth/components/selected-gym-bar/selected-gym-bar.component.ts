import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ListGymsQueryDto } from '../../../../api-services/gyms/gyms-api.models';
import { GymSelectionService } from '../../../public/services/gym-selection.service';

@Component({
  selector: 'app-selected-gym-bar',
  standalone: false,
  templateUrl: './selected-gym-bar.component.html',
  styleUrl: './selected-gym-bar.component.scss',
})
export class SelectedGymBarComponent implements OnInit {
  private readonly gymSelection = inject(GymSelectionService);
  private readonly router = inject(Router);

  gym: ListGymsQueryDto | null = null;

  ngOnInit(): void {
    this.gym = this.gymSelection.getSelectedGym();
  }

  changeGym(): void {
    this.gymSelection.clearSelection();
    this.router.navigate(['/']);
  }
}
