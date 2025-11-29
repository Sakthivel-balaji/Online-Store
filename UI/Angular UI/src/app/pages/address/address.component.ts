import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AddressModel } from '../../shared/models/address.model';
import { AddressService } from './address.service';
import { TokenDecodeService } from '../../shared/services/token-decode.service';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { AddeditaddressComponent } from './addeditaddress/addeditaddress.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})

export class AddressComponent implements OnInit {
  loading: boolean = false;
  dataAPI: AddressModel[] = [];
  dataUI: AddressModel = {};
  customerIdfromToken: number = 0;
  isAddressAccessedByCorrectUser: boolean = false;
  selectedAddressId: number = 0;

  @Input() customerId: number = 0;
  @Input() isAccessedFromCustomers: boolean = false;
  @Output() selectedAddressForOrder = new EventEmitter();

  constructor(
    private addressAPIService: AddressService,
    private tokenDecodeService: TokenDecodeService,
    private popupService: PopupmessageService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.customerIdfromToken = Number(this.tokenDecodeService.getCustomerId());
    this.isAddressAccessedByCorrectUser = this.customerIdfromToken === this.customerId;
    this.getAddressesByCustomerId();
  }

  getAddressesByCustomerId() {
    this.loading = true;

    this.addressAPIService.GetByCustomerId(this.customerId).subscribe(
      res => {
        this.dataAPI = res;
        this.loading = false;
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong while fetching reviews");
      }
    );
  }

  insertAddress() {
    this.loading = true;

    this.addressAPIService.InsertAddress(this.dataUI).subscribe(
      res => {
        this.loading = false;
        this.popupService.showSuccessMessage("Address inserted successfully");
        this.getAddressesByCustomerId();
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  updateAddress() {
    this.loading = true;

    this.addressAPIService.UpdateAddress(this.dataUI).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode == 400) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Address updated successfully");
          this.getAddressesByCustomerId();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  deleteAddress(addressId: any) {
    this.loading = true;

    this.addressAPIService.DeleteAddress(addressId).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode == 400) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Address deleted successfully");
          this.getAddressesByCustomerId();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  confirmDelete(addressId: any): void {
    if (confirm('Are you sure you want to delete this address?')) {
      this.deleteAddress(addressId);
    }
  }

  openEditDialog(selectedCarddata: AddressModel): void {
    const dialogRef = this.dialog.open(AddeditaddressComponent, {
      width: '900px',
      data: { ...selectedCarddata }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.dataUI = result;
        if (result.addressId === 0 || !result.addressId) {
          this.insertAddress();
        } else {
          this.updateAddress();
        }
      }
    });
  }

  emitAddressId() {
    this.selectedAddressForOrder.emit(this.selectedAddressId);
  }
}
