import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { UserProfileService } from '../../../core/services/user-profile.service';

@Component({
  selector: 'app-admin-layout',
  standalone: false,
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent implements OnInit {
  auth = inject(AuthFacadeService);
  profileService = inject(UserProfileService);
  private router = inject(Router);

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe();
  }

  logout(): void {
    this.profileService.clear();
    this.router.navigate(['/auth/logout']);
  }
}
