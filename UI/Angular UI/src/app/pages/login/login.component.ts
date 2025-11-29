import { Component } from '@angular/core';
import { LoginService } from './login.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { LoginModel } from '../../shared/models/users.model';
import { Router } from '@angular/router';
import { TokenDecodeService } from '../../shared/services/token-decode.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})

export class LoginComponent {
  loginForm: FormGroup;
  loading: boolean = false;
  customerId: number = 0;
  role: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private loginService: LoginService,
    private popupService: PopupmessageService,
    private tokenDecodeService: TokenDecodeService,
    private router: Router 
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  login() {
    if (this.loginForm.invalid) {
      this.popupService.showErrorMessage("Enter Valid Details");
      return;
    }

    const loginPayload: LoginModel = {
      Email: this.loginForm.get("email")?.value,
      PasswordHash: this.loginForm.get("password")?.value
    };

    this.loading = true;
    this.loginService.Login(loginPayload).subscribe(
      res => {
        this.loading = false;
        localStorage.setItem('token', res.token);
        this.customerId = Number(this.tokenDecodeService.getCustomerId());
        this.role = this.tokenDecodeService.getUserRole();
        this.tokenDecodeService.setCustomerId(this.customerId);
        this.tokenDecodeService.setRole(this.role);
        this.popupService.showSuccessMessage("Logged In Successfully");
        this.router.navigate(['/products']);
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Invalid Credentials");
      }
    )
  }
}
