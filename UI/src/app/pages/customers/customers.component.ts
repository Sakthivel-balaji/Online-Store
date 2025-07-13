import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { CustomersService } from './customers.service';
import { CustomersModel } from '../../shared/models/customers.model';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})

export class CustomersComponent implements OnInit {
  constructor(
    private customerAPIService: CustomersService,
    private popupService: PopupmessageService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  loading = true;
  tableLoading = true;
  data: CustomersModel[] = [];
  params: any = {};
  sortby = 'CustomerId';
  sortorder = 'ASC';
  customerIdsdropdown: any[] = [];
  customerNamesdropdown: any[] = [];
  customerEmailsdropdown: any[] = [];
  CustomerId: string | null = null;
  FullName: string | null = null;
  Email: string | null = null;
  totalRecords = 0;

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.CustomerId = params['CustomerId'];
      this.FullName = params['FullName'];
      this.Email = params['Email'];
    });
    this.getDropdownValues();
  }

  getDropdownValues() {
    this.customerAPIService.GetDropdownValues().subscribe(
      res => {
        this.customerIdsdropdown = res.CustomerIds;
        this.customerNamesdropdown = res.CustomerNames;
        this.customerEmailsdropdown = res.CustomerEmails;
      },
      err => {
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong while fetching dropdowns");
      }
    )
  }

  getPaginatedCustomers(event: any) {
    if (event.sortField) {
      this.sortby = event.sortField;
      this.sortorder = event.sortOrder === 1 ? 'DESC' : 'ASC';
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { CustomerId: this.CustomerId, FullName: this.FullName, Email: this.Email },
      queryParamsHandling: 'merge'
    });

    let params = {
      CustomerId: this.CustomerId || '',
      FullName: this.FullName || '',
      Email: this.Email || '',
    };

    const jsonString = JSON.stringify(params);
    this.tableLoading = true;
    this.data = []

    this.params['pageNumber'] = event.first ? (event.first / event.rows) + 1 : 1;
    this.params['pageSize'] = event.rows || 10;
    this.params['sortColumn'] = this.sortby || 'CustomerId';
    this.params['sortOrder'] = this.sortorder || 'ASC';
    this.params['filterColumn'] = '';
    this.params['filterValue'] = jsonString || '';

    this.customerAPIService.GetAll(this.params).subscribe(
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

  previewCustomer(customerId: number) {
    this.router.navigate([`/customers/${customerId}`]);
  }

  clearCustomerId(){
    this.CustomerId = null;
  }

  clearFullName(){
    this.FullName = null;
  }

  clearEmail(){
    this.Email = null;
  }
}
