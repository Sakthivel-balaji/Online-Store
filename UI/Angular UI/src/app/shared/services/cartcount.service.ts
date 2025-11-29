import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class CartcountService {
  private cartCount = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCount.asObservable();

  constructor() { }

  setCartCount(count: number): void {
    this.cartCount.next(count);
  }

  incrementCartCount(): void {
    const currentCount = this.cartCount.value;
    this.cartCount.next(currentCount + 1);
  }

  decrementCartCount(): void {
    const currentCount = this.cartCount.value;
    if (currentCount > 0) {
      this.cartCount.next(currentCount - 1);
    }
  }

  resetCartCount(): void {
    this.cartCount.next(0);
  }

  incrementCartCountByNumber(quantity: number): void {
    const currentCount = this.cartCount.value;
    this.cartCount.next(currentCount + quantity);
  }

  decrementCartCountByNumber(quantity: number): void {
    const currentCount = this.cartCount.value;
    if (currentCount > 0) {
      this.cartCount.next(currentCount - quantity);
    }
  }
}
