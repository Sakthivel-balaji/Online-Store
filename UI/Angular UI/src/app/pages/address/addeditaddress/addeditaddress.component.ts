import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AddressModel } from '../../../shared/models/address.model';

@Component({
  selector: 'app-addeditaddress',
  templateUrl: './addeditaddress.component.html',
  styleUrls: ['./addeditaddress.component.css']
})

export class AddeditaddressComponent implements OnInit {
  customerForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AddeditaddressComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AddressModel,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.customerForm = this.fb.group({
      addressId: [this.data.addressId || 0],
      customerId: [this.data.customerId || 0, Validators.required],
      address: [this.data.address || '', Validators.required],
      city: [this.data.city || '', Validators.required],
      state: [this.data.state || '', Validators.required],
      country: [this.data.country || '', Validators.required],
      postalCode: [this.data.postalCode || '', Validators.required],
      phone: [this.data.phone || '', Validators.required],
      isPrimary: [this.data.isPrimary || false, Validators.required],
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.customerForm.valid) {
      const formValue = { ...this.customerForm.value };
      formValue.customerId = this.data.customerId;
      this.dialogRef.close(formValue);
    }
  }
}
