import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { LayoutComponent } from './layout/layout.component';
import { ProductsComponent } from './pages/products/products.component';
import { AddeditproductComponent } from './pages/products/addeditproduct/addeditproduct.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { CartComponent } from './pages/cart/cart.component';
import { AddeditorderComponent } from './pages/orders/addeditorder/addeditorder.component';
import { AddeditcustomerComponent } from './pages/customers/addeditcustomer/addeditcustomer.component';
import { PermissionComponent } from './pages/permission/permission.component';
import { ErrorComponent } from './pages/error/error.component';
import { AdminGuard } from './routeguards/admin.guard';
import { CustomerGuard } from './routeguards/customer.guard';
import { AuthGuard } from './routeguards/auth.guard';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'products', pathMatch: 'full' },
            { path: 'products', component: ProductsComponent },
            { path: 'products/:id', component: AddeditproductComponent },
            { path: 'orders', component: OrdersComponent, canActivate: [AuthGuard] },
            { path: 'orders/:id', component: AddeditorderComponent, canActivate: [AuthGuard] },
            { path: 'customers', component: CustomersComponent, canActivate: [AdminGuard] },
            { path: 'customers/:id', component: AddeditcustomerComponent, canActivate: [CustomerGuard] },
            { path: 'cart', component: CartComponent, canActivate: [AuthGuard] },
        ],
    },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'permission', component: PermissionComponent, pathMatch: "full" },
    { path: '**', component: ErrorComponent, pathMatch: "full" },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})

export class AppRoutingModule { }
