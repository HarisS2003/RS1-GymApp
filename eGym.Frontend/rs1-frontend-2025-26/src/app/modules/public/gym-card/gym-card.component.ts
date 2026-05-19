import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ListGymsQueryDto } from '../../../api-services/gyms/gyms-api.models';

@Component({
  selector: 'app-gym-card',
  standalone: false,
  templateUrl: './gym-card.component.html',
  styleUrl: './gym-card.component.scss',
})
export class GymCardComponent {
  @Input({ required: true }) gym!: ListGymsQueryDto;
  @Input({ required: true }) accentIndex = 0;
  @Output() gymSelected = new EventEmitter<ListGymsQueryDto>();

  onSelect(): void {
    this.gymSelected.emit(this.gym);
  }
}
