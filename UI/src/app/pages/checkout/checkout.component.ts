import { Component, Inject, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})

export class CheckoutComponent {
  constructor(
    public dialogRef: MatDialogRef<CheckoutComponent>,
    @Inject(MAT_DIALOG_DATA) public customerId: number,
  ) { }

  @Input() addressId: number = 0;

  storeAddressId(addressId: number) {
    this.addressId = addressId;
  }

  placeOrder() {
    this.dialogRef.close(this.addressId);
  }
}
