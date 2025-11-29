// Componenents
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ProductsComponent } from './pages/products/products.component';
import { AddeditproductComponent } from './pages/products/addeditproduct/addeditproduct.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { AddeditorderComponent } from './pages/orders/addeditorder/addeditorder.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { AddeditcustomerComponent } from './pages/customers/addeditcustomer/addeditcustomer.component';
import { AddressComponent } from './pages/address/address.component';
import { AddeditaddressComponent } from './pages/address/addeditaddress/addeditaddress.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { ErrorComponent } from './pages/error/error.component';
import { PermissionComponent } from './pages/permission/permission.component';
import { ReviewsComponent } from './pages/reviews/reviews.component';
import { EditProductDialogComponent } from './pages/products/edit-product-dialog/edit-product-dialog.component';
import { LayoutComponent } from './layout/layout.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';

// Angular Material
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSidenavModule } from "@angular/material/sidenav"
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCommonModule, MatOptionModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';

// PrimeNg
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { KeyFilterModule } from 'primeng/keyfilter';
import { MessageModule } from 'primeng/message';
import { MultiSelectModule } from 'primeng/multiselect';
import { MessagesModule } from 'primeng/messages';
import { CalendarModule } from 'primeng/calendar';
import { SplitterModule } from 'primeng/splitter';
import { AccordionModule } from 'primeng/accordion';
import { ToastModule } from 'primeng/toast';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { FileUploadModule } from 'primeng/fileupload';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SpeedDialModule } from 'primeng/speeddial';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';

// Others
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app.routes';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    LayoutComponent,
    NavbarComponent,
    FooterComponent,
    ProductsComponent,
    AddeditproductComponent,
    OrdersComponent,
    AddeditorderComponent,
    CustomersComponent,
    AddeditcustomerComponent,
    CartComponent,
    CheckoutComponent,
    AddressComponent,
    AddeditaddressComponent,
    ErrorComponent,
    PermissionComponent,
    ReviewsComponent,
    EditProductDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    RouterModule,
    InputTextareaModule,
    TableModule,
    DropdownModule,
    InputTextModule,
    KeyFilterModule,
    MessageModule,
    MultiSelectModule,
    MessagesModule,
    CalendarModule,
    SplitterModule,
    AccordionModule,
    ToastModule,
    InputSwitchModule,
    ToggleButtonModule,
    FileUploadModule,
    RadioButtonModule,
    InputNumberModule,
    CheckboxModule,
    SpeedDialModule,
    MatSnackBarModule,
    MatSidenavModule,
    MatMenuModule,
    MatTabsModule,
    MatTooltipModule,
    MatDialogModule,
    MatCommonModule, 
    MatOptionModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatChipsModule,
    MatCheckboxModule,
    MatToolbarModule,
    MatCardModule,
    NgxSliderModule,
    MatInputModule,
    MatButtonModule
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
