import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenDecodeService } from '../shared/services/token-decode.service';

@Injectable({
  providedIn: 'root',
})

export class AdminGuard {
  constructor(
    private router: Router,
    private tokenService: TokenDecodeService
  ) {}

  canActivate(): boolean {
    const role = this.tokenService.getUserRole();
    if (role !== 'Admin') {
      this.router.navigate(['/permission']);
      return false;
    }
    return true;
  }
}
