import { Component, OnInit } from '@angular/core';
import { TokenDecodeService } from '../../../shared/services/token-decode.service';
import { PopupmessageService } from '../../../shared/services/popupmessage.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrdersService } from '../orders.service';
import { OrderModel, OrderUpdateModel, ProductInfoModel } from '../../../shared/models/orders.model';

@Component({
  selector: 'app-addeditorder',
  templateUrl: './addeditorder.component.html',
  styleUrls: ['./addeditorder.component.css']
})

export class AddeditorderComponent implements OnInit {
  role: string = '';
  customerId: number = 0;
  orderId: number = 0;
  loading: boolean = false;
  editAccess: boolean = false;
  status: string = '';
  deliveryDate: string = '';
  dataAPI: OrderModel = {} as OrderModel;

  paginatedProducts: ProductInfoModel[] = [];
  currentPage: number = 0;
  itemsPerPage = 3;

  constructor(
    private ordersAPIService: OrdersService,
    private tokenDecodeService: TokenDecodeService,
    private popupService: PopupmessageService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.role = this.tokenDecodeService.getUserRole();
    this.customerId = Number(this.tokenDecodeService.getCustomerId());
    this.route.params.subscribe(params => {
      this.orderId = Number(params['id']);
    });
    this.editAccess = this.role === 'Admin';
    this.fetchOrderDetails();
  }

  fetchOrderDetails() {
    this.loading = true;
    this.ordersAPIService.GetByOrderId(this.orderId).subscribe(
      res => {
        this.dataAPI = res;
        this.status = res.status ?? '';
        this.deliveryDate = res.deliveryDate ? this.formatDate(res.deliveryDate) : new Date().toISOString().split('T')[0];
        this.paginate(0);
        this.loading = false;
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  editOrder() {
    if (!this.status || !this.deliveryDate) {
      this.popupService.showErrorMessage("Please fill Status and Delivery Date");
      return;
    }

    this.loading = true;

    const orderPayload: OrderUpdateModel = {
      orderId: this.orderId,
      status: this.status,
      deliveryDate: new Date(this.deliveryDate)
    };

    this.ordersAPIService.UpdateOrder(orderPayload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Order updated successfully");
          this.fetchOrderDetails();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  deleteOrder() {
    if (confirm("Are you sure you want to delete this order?")) {
      this.loading = true;

      this.ordersAPIService.DeleteOrder(this.orderId).subscribe(
        res => {
          this.loading = false;
          if (res.statusCode != 200) {
            this.popupService.showErrorMessage(res.message);
          }
          else {
            this.popupService.showSuccessMessage("Order deleted successfully");
            this.router.navigate(['/orders']);
          }
        },
        err => {
          this.loading = false;
          console.log(err);
          this.popupService.showErrorMessage("Something went wrong");
        }
      );
    }
  }

  formatDate(sqlDate: string): string {
    const date = new Date(sqlDate);
    return date.toISOString().split('T')[0];
  }

  redirectToProduct(productId: number) {
    this.router.navigate([`/products/${productId}`]);
  }

  paginate(direction: number) {
    if (!this.dataAPI || !this.dataAPI.products) return;

    this.currentPage += direction;
    let startIndex = this.currentPage * this.itemsPerPage;
    this.paginatedProducts = this.dataAPI.products.slice(startIndex, startIndex + this.itemsPerPage);
  }

  getMaxPage(): number {
    return Math.ceil((this.dataAPI.products?.length ?? 0) / this.itemsPerPage) - 1;
  }
}
