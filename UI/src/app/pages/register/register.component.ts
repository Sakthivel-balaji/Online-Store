import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RegisterService } from './register.service';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { Router } from '@angular/router';
import { RegisterModel } from '../../shared/models/users.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent {
  registerForm: FormGroup;
  loading: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private registerService: RegisterService,
    private popupService: PopupmessageService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_]+$')]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  register() {
    if (this.registerForm.invalid) {
      this.popupService.showErrorMessage("Enter Valid Details");
      return;
    }

    const registerPayload: RegisterModel = {
      UserName: this.registerForm.get("username")?.value,
      Email: this.registerForm.get("email")?.value,
      PasswordHash: this.registerForm.get("password")?.value
    };

    this.loading = true;
    this.registerService.Register(registerPayload).subscribe(
      res => {
        this.loading = false;
        this.popupService.showSuccessMessage("Registered Successfully");
        this.router.navigate(['/login']);
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage(err.error?.message || "Something went wrong!!!");
      }
    )
  }
}
