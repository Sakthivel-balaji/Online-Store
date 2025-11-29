import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class TokenDecodeService {
  private customerId = new BehaviorSubject<number>(0);
  customerId$ = this.customerId.asObservable();

  private role = new BehaviorSubject<string>('');
  role$ = this.role.asObservable();
  
  private payload: any;

  public initialize(): void {
    const token = localStorage.getItem('token');

    if (!token || token.split('.').length !== 3) {
      console.warn('No valid JWT token found to decode.');
      this.payload = null;
      return;
    }

    try {
      this.payload = jwtDecode(token);
    } catch (err) {
      console.error('Failed to decode token in initialize()', err);
      this.payload = null;
    }
  }

  private decodeToken(): any {
    const token = localStorage.getItem('token');
    if (!token || token.split('.').length !== 3) return null;

    try {
      return jwtDecode(token);
    } catch (err) {
      console.error('Invalid JWT token', err);
      return null;
    }
  }

  private getPayload(): any {
    if (!this.payload) {
      this.payload = this.decodeToken();
    }
    return this.payload;
  }

  getUserRole(): string {
    const payload = this.getPayload();
    return payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
  }

  getCustomerId(): string {
    const payload = this.getPayload();
    return payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || '';
  }

  clearPayload() {
    this.payload = null;
  }

  setCustomerId(id: number): void {
    this.customerId.next(id);
  }

  resetCustomerId(): void {
    this.customerId.next(0);
  }

  setRole(role: string): void {
    this.role.next(role);
  }

  resetRole(): void {
    this.role.next('');
  }
}
