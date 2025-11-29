import { Component, OnInit } from '@angular/core';
import { CustomersService } from '../customers.service';
import { TokenDecodeService } from '../../../shared/services/token-decode.service';
import { PopupmessageService } from '../../../shared/services/popupmessage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerModel, CustomerUpdateModel } from '../../../shared/models/customers.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-addeditcustomer',
  templateUrl: './addeditcustomer.component.html',
  styleUrl: './addeditcustomer.component.css'
})

export class AddeditcustomerComponent implements OnInit {
  role: string = '';
  customerId: number = 0;
  loading: boolean = false;
  editAccess: boolean = false;
  isEditing = false;
  dateaFromUI: CustomerModel = {} as CustomerModel;
  dataFromAPI: CustomerModel = {} as CustomerModel;
  customerForm!: FormGroup;

  constructor(
    private customersAPIService: CustomersService,
    private tokenDecodeService: TokenDecodeService,
    private popupService: PopupmessageService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.role = this.tokenDecodeService.getUserRole();
    this.initializeForm();

    this.route.params.subscribe(params => {
      this.customerId = Number(params['id']);
    });

    this.editAccess = this.customerId == Number(this.tokenDecodeService.getCustomerId()) ? true : false;
    this.getCutomerInfo();
  }

  initializeForm(): void {
    this.customerForm = this.fb.group({
      customerId: [this.dataFromAPI.customerId || 0],
      email: [this.dataFromAPI.email || '', Validators.required],
      profilePicture: [this.dataFromAPI.profilePicture || null],
      fullName: [this.dataFromAPI.fullName || '', Validators.required],
      phone: [this.dataFromAPI.phone || '', Validators.required]
    });
  }

  getCutomerInfo() {
    this.loading = true;
    this.customersAPIService.GetByCustomerId(this.customerId).subscribe(
      res => {
        this.dataFromAPI = res;
        this.loading = false;
        this.initializeForm();
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  editCutomerInfo() {
    this.loading = true;

    const customerPayload: CustomerUpdateModel = {
      customerId: this.customerId,
      profilePicture: this.customerForm.get("profilePicture")?.value,
      fullName: this.customerForm.get("fullName")?.value,
      phone: this.customerForm.get("phone")?.value,
    }

    this.customersAPIService.UpdateCustomer(customerPayload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Details updated successfully");
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  deleteCustomer() {
    this.loading = true;
    this.customersAPIService.DeleteCustomer(this.customerId).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Customer deleted successfully");
          this.router.navigate(['/customers']);
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  onImageSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        const base64String = (reader.result as string).split(',')[1];
        this.customerForm.patchValue({ profilePicture: base64String });
      };
      reader.readAsDataURL(file);
    }
  }

  toggleEdit() {
    this.isEditing = true;
  }

  cancelEdit() {
    this.isEditing = false;
    this.initializeForm();
  }

  saveChanges() {
    if (this.customerForm.valid) {
      this.editCutomerInfo();
      this.isEditing = false;
    }
  }
}
