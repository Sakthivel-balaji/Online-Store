import { Component, OnInit } from '@angular/core';
import { TokenDecodeService } from '../../../shared/services/token-decode.service';
import { CartService } from '../../cart/cart.service';
import { CartcountService } from '../../../shared/services/cartcount.service';
import { PopupmessageService } from '../../../shared/services/popupmessage.service';
import { ProductsService } from '../products.service';
import { ProductModel } from '../../../shared/models/products.model';
import { ActivatedRoute, Router } from '@angular/router';
import { CartInsertModel } from '../../../shared/models/cart.model';
import { MatDialog } from '@angular/material/dialog';
import { EditProductDialogComponent } from '../edit-product-dialog/edit-product-dialog.component';

@Component({
  selector: 'app-addeditproduct',
  templateUrl: './addeditproduct.component.html',
  styleUrls: ['./addeditproduct.component.css']
})

export class AddeditproductComponent implements OnInit {

  role: string = '';
  customerId: number = 0;
  loading: boolean = false;
  editAccess: boolean = false;
  cardData: ProductModel = {} as ProductModel;
  data: ProductModel = {} as ProductModel;
  token: string | null = '';
  productId: number = 0;
  quantity: number = 1;
  totalQuantity: number = 0;

  constructor(
    private productsAPIService: ProductsService,
    private tokenDecodeService: TokenDecodeService,
    private cartAPIService: CartService,
    private cartCountSubject: CartcountService,
    private popupService: PopupmessageService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.token = localStorage.getItem('token');
    this.role = this.tokenDecodeService.getUserRole();
    this.customerId = Number(this.tokenDecodeService.getCustomerId());

    this.route.params.subscribe(params => {
      this.productId = Number(params['id']);
    });

    this.editAccess = this.role === 'Admin';

    if (this.productId != 0) {
      this.fetchProductDetail();
    }
  }

  fetchProductDetail() {
    this.loading = true;
    this.productsAPIService.GetByProductId(this.productId).subscribe(
      res => {
        this.data = res;
        this.cardData = { ...res };
        this.loading = false;
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  addProduct() {
    this.loading = true;
    this.productsAPIService.InsertProduct(this.cardData).subscribe(
      res => {
        this.loading = false;
        this.popupService.showSuccessMessage("Product inserted successfully");
        this.fetchProductDetail();
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  editProduct() {
    this.loading = true;
    this.productsAPIService.UpdateProduct(this.cardData).subscribe(
      res => {
        this.loading = false;
        this.popupService.showSuccessMessage("Product updated successfully");
        this.fetchProductDetail();
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  deleteProduct() {
    if (confirm("Are you sure you want to delete this product?")) {
      this.loading = true;
      this.productsAPIService.DeleteProduct(this.productId).subscribe(
        res => {
          this.loading = false;
          this.popupService.showSuccessMessage("Product deleted successfully");
          this.router.navigate(['/products']);
        },
        err => {
          this.loading = false;
          console.log(err);
          this.popupService.showErrorMessage("Something went wrong");
        }
      );
    }
  }

  InsertItemInCart(): void {
    this.loading = true;

    const payload: CartInsertModel = {
      customerId: this.customerId,
      productId: this.productId,
      quantity: this.quantity
    };

    this.cartAPIService.InsertItem(payload).subscribe(
      res => {
        this.cartCountSubject.incrementCartCountByNumber(payload.quantity);
        this.loading = false;
        this.popupService.showSuccessMessage('Added to cart');
      },
      err => {
        console.error(err);
        this.loading = false;
        this.popupService.showErrorMessage('Failed to add to cart');
      }
    );
  }

  getDiscountedPrice(price: number, discount?: number): number {
    if (!discount) return price;
    return Math.round(price - (price * discount / 100));
  }

  openEditDialog(): void {
    const dialogRef = this.dialog.open(EditProductDialogComponent, {
      width: '900px',
      data: { ...this.data }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cardData = result;
        if (result.productId === 0 || !result.productId) {
          this.addProduct();
        } else {
          this.editProduct();
        }
      }
    });
  }
}
