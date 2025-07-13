import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterModel } from '../../shared/models/users.model';

@Injectable({
  providedIn: 'root'
})

export class RegisterService {
  constructor(
    private httpClient: HttpClient
  ) { }

  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Users;

  Register(credentials: RegisterModel): Observable<any> {
    return this.httpClient.post(this.OnlineStoreWebAPI + "register", credentials);
  }
}
