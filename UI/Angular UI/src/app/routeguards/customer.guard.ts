import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { TokenDecodeService } from '../shared/services/token-decode.service';

@Injectable({
  providedIn: 'root',
})

export class CustomerGuard {
  constructor(
      private router: Router,
      private tokenService: TokenDecodeService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const userRole = this.tokenService.getUserRole();

    if(userRole == "Admin")
    {
       return true;
    }

    const customerIdInParam = route.params['id'];
    const loggedInCustomerId = this.tokenService.getCustomerId();
    
    if (customerIdInParam !== loggedInCustomerId) {
      this.router.navigate(['/permission']);
      return false;
    }
    return true;
  }
}
