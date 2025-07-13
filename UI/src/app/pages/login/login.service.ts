import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginModel } from '../../shared/models/users.model';

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  constructor(
    private httpClient: HttpClient
  ) { }

  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Users;

  Login(credentials: LoginModel): Observable<any> {
    return this.httpClient.post(this.OnlineStoreWebAPI + "login", credentials);
  }
}
