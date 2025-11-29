import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CartInsertModel, CartUpdateModel } from '../../shared/models/cart.model';

@Injectable({
  providedIn: 'root'
})

export class CartService {
  constructor(
    private httpClient: HttpClient
  ) { }

  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Cart;

  GetByCustomerId(customerId: number): Observable<any> {
    const token = "Bearer " + localStorage.getItem("token");
    var headers = new HttpHeaders({ 'Authorization': token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + customerId, { headers: headers });
  }

  InsertItem(cart: CartInsertModel): Observable<any> {
    const token = "Bearer " + localStorage.getItem("token");
    var headers = new HttpHeaders({ 'Authorization': token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI, cart, { headers: headers });
  }

  UpdateItem(cart: CartUpdateModel): Observable<any> {
    const token = "Bearer " + localStorage.getItem("token");
    var headers = new HttpHeaders({ 'Authorization': token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.patch(this.OnlineStoreWebAPI, cart, { headers: headers });
  }

  DeleteItem(cartId: number): Observable<any> {
    const token = "Bearer " + localStorage.getItem("token");
    var headers = new HttpHeaders({ 'Authorization': token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + cartId, { headers: headers });
  }
}
