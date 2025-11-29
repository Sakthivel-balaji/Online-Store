import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerUpdateModel } from '../../shared/models/customers.model';
import { PaginationRequest } from '../../shared/models/pagination.model';

@Injectable({
  providedIn: 'root'
})

export class CustomersService {
  constructor(
    private httpClient: HttpClient
  ) { }

  token: string = "Bearer " + localStorage.getItem("token");
  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Customers;

  GetDropdownValues(): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + "dropdown-values", { headers });
  }

  GetAll(paginationRequest: PaginationRequest): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI + "GetAll", paginationRequest, { headers });
  }

  GetByCustomerId(customerId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + customerId, { headers: headers });
  }

  UpdateCustomer(customer: CustomerUpdateModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.patch(this.OnlineStoreWebAPI, customer, { headers: headers });
  }

  DeleteCustomer(customerId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + customerId, { headers: headers });
  }
}
