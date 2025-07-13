import { Component, OnInit } from '@angular/core';
import { CartModel, CartProductsModel, CartUpdateModel } from '../../shared/models/cart.model';
import { CartService } from './cart.service';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { MatDialog } from '@angular/material/dialog';
import { TokenDecodeService } from '../../shared/services/token-decode.service';
import { CartcountService } from '../../shared/services/cartcount.service';
import { Router } from '@angular/router';
import { CheckoutComponent } from '../checkout/checkout.component';
import { OrderInsertModel } from '../../shared/models/orders.model';
import { OrdersService } from '../orders/orders.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})

export class CartComponent implements OnInit {
  customerId: number = 0;
  addressId: number = 0;
  loading = true;
  dataAPI: CartModel = {} as CartModel;
  dataUI: CartModel = {} as CartModel;
  cartCount: number = 0;
  addressIdForOrder: number = 0;
  totalQuantity: number = 0;

  paginatedProducts: CartProductsModel[] = [];
  currentPage: number = 0;
  itemsPerPage = 3;

  constructor(
    private tokenDecodeService: TokenDecodeService,
    private cartAPIService: CartService,
    private cartCountSubject: CartcountService,
    private popupService: PopupmessageService,
    private router: Router,
    private dialog: MatDialog,
    private orderService: OrdersService
  ) { }

  ngOnInit(): void {
    this.customerId = Number(this.tokenDecodeService.getCustomerId());
    this.getCartItems();
  }

  getCartItems() {
    this.loading = true;

    this.cartAPIService.GetByCustomerId(this.customerId).subscribe(
      (res: CartModel) => {
        this.dataAPI = res;
        this.totalQuantity = 0; 

        if (res.products) {
          res.products.forEach((product: CartProductsModel) => {
            this.totalQuantity += product.quantity;
          });
        }

        this.cartCountSubject.setCartCount(this.totalQuantity);
        this.cartCount = this.totalQuantity;
        this.paginate(0);
        this.loading = false;
      },
      err => {
        this.popupService.showErrorMessage("Something went wrong while fetching cart items");
        console.error(err);
        this.loading = false;
      }
    )
  }

  placeOrder() {
    this.loading = true;

    const orderPayload: OrderInsertModel = {
      customerId: this.customerId,
      addressId: this.addressIdForOrder
    }

    this.orderService.InsertOrder(orderPayload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Order has been placed successfully");
          this.router.navigate(['/orders']);
          this.cartCountSubject.resetCartCount();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage(err.error?.message || "Something went wrong!!!");
      }
    )
  }

  editCartItem(cartItemId: number, quantity: number) {
    if (quantity <= 0) {
      this.popupService.showErrorMessage("Quantity must be greater than zero");
      return;
    }

    this.loading = true;

    const cartPayload: CartUpdateModel = {
      cartItemId: cartItemId,
      quantity: quantity
    }

    this.cartAPIService.UpdateItem(cartPayload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Cart updated successfully");
          this.getCartItems();
        }
      },
      err => {
        this.popupService.showErrorMessage("Something went wrong while updating the cart item");
        console.error(err);
        this.loading = false;
      }
    )
  }

  deleteCartItem(cartId: number) {
    this.loading = true;

    this.cartAPIService.DeleteItem(cartId).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Cart item deleted successfully");
          this.getCartItems();
        }
      },
      err => {
        this.popupService.showErrorMessage("Something went wrong while deleting the cart item");
        console.error(err);
        this.loading = false;
      }
    )
  }

  paginate(direction: number) {
    if (!this.dataAPI || !this.dataAPI.products) return;

    this.currentPage += direction;
    let startIndex = this.currentPage * this.itemsPerPage;
    this.paginatedProducts = this.dataAPI.products.slice(startIndex, startIndex + this.itemsPerPage);
  }


  previewProduct(productId: number): void {
    this.router.navigate(['/products', productId]);
  }

  checkOut(): void {
    const dialogRef = this.dialog.open(CheckoutComponent, {
      width: '900px',
      height: 'auto',
      maxHeight: '90vh',
      data: this.customerId
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.addressIdForOrder = result;
        this.placeOrder();
      }
    });
  }


  getMaxPage(): number {
    return Math.floor((this.dataAPI.products?.length ?? 0) / this.itemsPerPage);
  }
}
