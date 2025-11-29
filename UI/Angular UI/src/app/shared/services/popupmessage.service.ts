import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})

export class PopupmessageService {
  constructor(private snackbar: MatSnackBar) { }

  showSuccessMessage(message: any, duration = 2000) {
    this.snackbar.open(message, undefined, {
      duration: duration,
      verticalPosition: 'bottom',
      horizontalPosition: 'right',
      panelClass: ["mat-success"]
    });
  }

  showErrorMessage(message: any, duration = 5000) {
    this.snackbar.open(message, undefined, {
      duration: duration,
      verticalPosition: 'bottom',
      horizontalPosition: 'right',
      panelClass: ["mat-error"]
    });
  }

  showInfoMessage(message: any, duration = 2000) {
    this.snackbar.open(message, undefined, {
      duration: duration,
      verticalPosition: 'bottom',
      horizontalPosition: 'right',
      panelClass: ['mat-info']
    });
  }

  showWarningMessage(message: any, duration = 2000) {
    this.snackbar.open(message, undefined, {
      duration: duration,
      verticalPosition: 'bottom',
      horizontalPosition: 'right',
      panelClass: ['mat-warning']
    });
  }
}
