import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginationRequest } from '../../shared/models/pagination.model';
import { OrderInsertModel, OrderUpdateModel } from '../../shared/models/orders.model';

@Injectable({
  providedIn: 'root'
})

export class OrdersService {
  constructor(
    private httpClient: HttpClient
  ) { }

  token: string = "Bearer " + localStorage.getItem("token");
  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Orders;

  GetDropdownValues(): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + "dropdown-values", { headers });
  }

  GetOrders(paginationRequest: PaginationRequest): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI + "GetOrders", paginationRequest, { headers });
  }

  InsertOrder(order: OrderInsertModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI, order, { headers: headers });
  }

  GetByOrderId(orderId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + orderId, { headers: headers });
  }

  UpdateOrder(order: OrderUpdateModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.patch(this.OnlineStoreWebAPI, order, { headers: headers });
  }

  DeleteOrder(orderId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + orderId, { headers: headers });
  }
}
