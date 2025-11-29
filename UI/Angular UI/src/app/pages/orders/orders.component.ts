import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { OrdersService } from './orders.service';
import { OrdersModel } from '../../shared/models/orders.model';
import { TokenDecodeService } from '../../shared/services/token-decode.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})

export class OrdersComponent implements OnInit{
  constructor(
    private ordersAPIService: OrdersService,
    private popupService: PopupmessageService,
    private tokenDecodeService: TokenDecodeService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  role: string = '';
  loading = true;
  tableLoading = true;
  data: OrdersModel[] = [];
  params: any = {};
  sortby = 'OrderDate';
  sortorder = 'DESC';
  OrderIdsdropdown: any[] = [];
  CustomerIdsdropdown: any[] = [];
  Emailsdropdown: any[] = [];
  OrderId: string | null = null;
  CustomerId: string | null = null;
  Email: string | null = null;
  totalRecords = 0;

  ngOnInit() {
    this.role = this.tokenDecodeService.getUserRole();
    if(this.role!='Admin') {
      return;
    }

    this.route.queryParams.subscribe((params) => {
      this.OrderId = params['OrderId'];
      this.CustomerId = params['CustomerId'];
      this.Email = params['Email'];
    });
    this.getDropdownValues();
  }

  getDropdownValues() {
    this.ordersAPIService.GetDropdownValues().subscribe(
      res => {
        this.OrderIdsdropdown = res.OrderIds;
        this.CustomerIdsdropdown = res.CustomerIds;
        this.Emailsdropdown = res.Emails;
      },
      err => {
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong while fetching dropdowns");
      }
    )
  }

  getPaginatedOrders(event: any) {
    if (event.sortField) {
      this.sortby = event.sortField;
      this.sortorder = event.sortOrder === 1 ? 'DESC' : 'ASC';
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { OrderId: this.OrderId, CustomerId: this.CustomerId, Email: this.Email },
      queryParamsHandling: 'merge'
    });

    let params = {
      OrderId: this.OrderId || '',
      CustomerId: this.role!='Admin' ? this.tokenDecodeService.getCustomerId() : this.CustomerId || '',
      Email: this.Email || '',
    };

    const jsonString = JSON.stringify(params);
    this.tableLoading = true;
    this.data = []

    this.params['pageNumber'] = event.first ? (event.first / event.rows) + 1 : 1;
    this.params['pageSize'] = event.rows || 10;
    this.params['sortColumn'] = this.sortby || 'OrderDate';
    this.params['sortOrder'] = this.sortorder || 'DESC';
    this.params['filterColumn'] = '';
    this.params['filterValue'] = jsonString || '';

    this.ordersAPIService.GetOrders(this.params).subscribe(
      (res: any) => {
        this.data = res.items || [];
        this.totalRecords = res.page.recordCount || 0;
        this.loading = false;
        this.tableLoading = false;
      },
      (error: any) => {
        console.log(error);
        this.popupService.showWarningMessage('Something went wrong try again');
        this.loading = false;
        this.tableLoading = false;
      }
    )
  }

  previewOrder(orderId: number) {
    this.router.navigate([`/orders/${orderId}`]);
  }

  clearOrderId() {
    this.OrderId = null;
  }

  clearCustomerId() {
    this.CustomerId = null;
  }

  clearEmail() {
    this.Email = null;
  }
}
