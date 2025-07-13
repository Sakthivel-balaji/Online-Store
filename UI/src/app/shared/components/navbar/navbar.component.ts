import { Component, OnInit } from '@angular/core';
import { CartcountService } from '../../services/cartcount.service';
import { TokenDecodeService } from '../../services/token-decode.service';
import { CartService } from '../../../pages/cart/cart.service';
import { PopupmessageService } from '../../services/popupmessage.service';
import { Router } from '@angular/router';
import { CartModel, CartProductsModel } from '../../models/cart.model';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})

export class NavbarComponent implements OnInit {
  constructor(
    private cartCountSubject: CartcountService,
    private tokenDecode: TokenDecodeService,
    private cartAPIservice: CartService,
    private popupService: PopupmessageService,
    private router: Router
  ) { }

  isLoggedIn: boolean = false;
  role: string = "";
  customerId: number = 0;
  cartCount: number = 0;
  token: any;
  totalQuantity: number = 0;

  ngOnInit(): void {
    this.role = this.tokenDecode.getUserRole();
    this.customerId = Number(this.tokenDecode.getCustomerId());
    this.token = localStorage.getItem('token');
    this.isLoggedIn = this.token != null ? true : false;
    this.cartCountSubject.cartCount$.subscribe(cartCount => this.cartCount = cartCount);

    if (this.token) {
      this.cartAPIservice.GetByCustomerId(this.customerId).subscribe(
        (res: CartModel) => {
          
          if (res.products) {
            res.products.forEach((product: CartProductsModel) => {
              this.totalQuantity += product.quantity;
            });
          }

          this.cartCountSubject.setCartCount(this.totalQuantity);
          this.cartCount = this.totalQuantity;
        },
        err => {
          console.error('Error fetching cart data:', err);
        }
      );
    }
  }

  logout() {
    this.role = "";
    this.customerId = 0;
    this.isLoggedIn = false;
    this.cartCount = 0;

    localStorage.clear();
    this.cartCountSubject.resetCartCount();
    this.tokenDecode.clearPayload();
    this.tokenDecode.resetCustomerId();
    this.tokenDecode.resetRole();
    this.router.navigate(['/products']);
    this.popupService.showSuccessMessage("Logged Out Successfully");
  }
}
