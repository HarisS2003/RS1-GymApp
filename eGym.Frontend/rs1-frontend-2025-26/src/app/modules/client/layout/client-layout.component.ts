import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { UserProfileService } from '../../../core/services/user-profile.service';

@Component({
  selector: 'app-client-layout',
  standalone: false,
  templateUrl: './client-layout.component.html',
  styleUrl: './client-layout.component.scss',
})
export class ClientLayoutComponent implements OnInit {
  auth = inject(AuthFacadeService);
  profileService = inject(UserProfileService);
  private router = inject(Router);

  profile = this.profileService.profile;
  isTrainer = this.profileService.isTrainer;

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe();
  }

  logout(): void {
    this.profileService.clear();
    this.router.navigate(['/auth/logout']);
  }
}
