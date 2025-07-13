import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginationRequest } from '../../shared/models/pagination.model';
import { ProductModel } from '../../shared/models/products.model';

@Injectable({
  providedIn: 'root'
})

export class ProductsService {
  constructor(
    private httpClient: HttpClient
  ) { }

  token: string = "Bearer " + localStorage.getItem("token");
  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Products;

  GetDropdownValues(): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + "dropdown-values", { headers });
  }

  GetAll(paginationRequest: PaginationRequest): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI + "GetAll", paginationRequest, { headers });
  }

  InsertProduct(product: ProductModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI, product, { headers: headers });
  }

  GetByProductId(productId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + productId, { headers: headers });
  }

  UpdateProduct(product: ProductModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.patch(this.OnlineStoreWebAPI, product, { headers: headers });
  }

  DeleteProduct(productId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + productId, { headers: headers });
  }
}
