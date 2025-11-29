import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { AddressModel } from '../../shared/models/address.model';

@Injectable({
  providedIn: 'root'
})

export class AddressService {
  constructor(
    private httpClient: HttpClient
  ) { }

  token: string = "Bearer " + localStorage.getItem("token");
  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Address;

  GetByCustomerId(customerId : number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.get(this.OnlineStoreWebAPI + customerId, { headers: headers });
  }

  InsertAddress(address: AddressModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI, address, { headers: headers });
  }

  UpdateAddress(address: AddressModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.patch(this.OnlineStoreWebAPI, address, { headers: headers });
  }

  DeleteAddress(addressId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + addressId, { headers: headers });
  }
}
