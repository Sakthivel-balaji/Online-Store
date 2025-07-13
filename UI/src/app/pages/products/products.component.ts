import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { ProductsService } from './products.service';
import { ProductModel, ProductsModel } from '../../shared/models/products.model';
import { TokenDecodeService } from '../../shared/services/token-decode.service';
import { CartService } from '../cart/cart.service';
import { CartcountService } from '../../shared/services/cartcount.service';
import { CartInsertModel } from '../../shared/models/cart.model';
import { Options } from '@angular-slider/ngx-slider';
import { MatDialog } from '@angular/material/dialog';
import { EditProductDialogComponent } from './edit-product-dialog/edit-product-dialog.component';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})

export class ProductsComponent implements OnInit {
  role: string = '';
  loading = true;
  data: ProductsModel[] = [];
  params: any = {};
  sortby = 'ProductId';
  sortorder = 'ASC';
  totalRecords = 0;
  customerId: number = 0;
  cartCount: number = 0;
  token: string | null = '';
  cardData: ProductModel = {} as ProductModel;

  categories: string[] = [];
  brands: string[] = [];
  minPriceRange = 0;
  maxPriceRange = 0;

  BrandFilter: string[] | null = null;
  CategoryFilter: string[] | null = null;
  MinPriceRangeFilter: string | null = null;
  MaxPriceRangeFilter: string | null = null;

  pageSize = 9;
  currentPage = 1;
  totalPages: number[] = [];

  minPriceRangeFilter: number = 0;
  maxPriceRangeFilter: number = 0;
  hoveredProduct: number | null = null;

  priceSliderOptions: Options = {
    floor: 0,
    ceil: 10000,
    step: 10
  };

  constructor(
    private productsAPIService: ProductsService,
    private tokenDecodeService: TokenDecodeService,
    private cartService: CartService,
    private cartCountSubject: CartcountService,
    private popupService: PopupmessageService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.role = this.tokenDecodeService.getUserRole();
    this.customerId = Number(this.tokenDecodeService.getCustomerId());
    this.token = localStorage.getItem('token');
    this.cartCountSubject.cartCount$.subscribe(count => this.cartCount = count);

    this.route.queryParams.subscribe(params => {
      this.BrandFilter = params['Brand'] ? params['Brand'].split(',') : null;
      this.CategoryFilter = params['Category'] ? params['Category'].split(',') : null;
      this.MinPriceRangeFilter = params['MinPriceRange'];
      this.MaxPriceRangeFilter = params['MaxPriceRange'];
    });

    this.getDropdownValues();
    this.getPaginatedProducts({});
  }

  getDropdownValues(): void {
    this.productsAPIService.GetDropdownValues().subscribe(
      res => {
        this.categories = res.Categories || [];
        this.brands = res.Brands || [];
        this.minPriceRangeFilter = +res.PriceRange[0];
        this.maxPriceRangeFilter = +res.PriceRange[1];

        this.priceSliderOptions = {
          floor: this.minPriceRangeFilter,
          ceil: this.maxPriceRangeFilter,
          step: 1
        };
      },
      err => {
        console.error(err);
        this.popupService.showErrorMessage('Failed to load filters');
      }
    );
  }

  getPaginatedProducts(event: any): void {
    if (event.sortField) {
      this.sortby = event.sortField;
      this.sortorder = event.sortOrder === 1 ? 'DESC' : 'ASC';
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        Brand: this.BrandFilter?.join(','),
        Category: this.CategoryFilter?.join(','),
        MinPriceRange: this.MinPriceRangeFilter,
        MaxPriceRange: this.MaxPriceRangeFilter,
        page: this.currentPage
      },
      queryParamsHandling: 'merge'
    });

    const filters = {
      Brands: this.BrandFilter || [],
      Categories: this.CategoryFilter || [],
      MinPriceRange: this.MinPriceRangeFilter || '',
      MaxPriceRange: this.MaxPriceRangeFilter || ''
    };

    this.params = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: this.sortby,
      sortOrder: this.sortorder,
      filterColumn: '',
      filterValue: JSON.stringify(filters)
    };

    this.loading = true;

    this.productsAPIService.GetAll(this.params).subscribe(
      res => {
        this.data = res.items || [];
        this.totalRecords = res.page.recordCount || 0;
        this.totalPages = Array(Math.ceil(this.totalRecords / this.pageSize)).fill(0).map((_, i) => i + 1);
        this.loading = false;
      },
      err => {
        console.error(err);
        this.popupService.showWarningMessage('Error loading products');
        this.loading = false;
      }
    );
  }

  previewProduct(productId: number): void {
    this.router.navigate(['/products', productId]);
  }

  InsertItemInCart(productId: number): void {
    this.loading = true;

    if (!this.token) {
      this.popupService.showInfoMessage('Please login to add item to cart');
      this.loading = false;
      return;
    }

    const payload: CartInsertModel = {
      customerId: this.customerId,
      productId: productId,
      quantity: 1
    };

    this.cartService.InsertItem(payload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.cartCountSubject.incrementCartCount();
          this.popupService.showSuccessMessage('Added to cart');
        }
      },
      err => {
        console.error(err);
        this.loading = false;
        this.popupService.showErrorMessage('Failed to add to cart');
      }
    );
  }

  onBrandFilterChange(event: any): void {
    const selectedBrand = event.target.value;
    if (event.target.checked) {
      if (!this.BrandFilter) this.BrandFilter = [];
      this.BrandFilter.push(selectedBrand);
    } else {
      this.BrandFilter = this.BrandFilter?.filter(brand => brand !== selectedBrand) || null;
    }
    this.getPaginatedProducts({});
  }

  onCategoryFilterChange(event: any): void {
    const selectedCategory = event.target.value;
    if (event.target.checked) {
      if (!this.CategoryFilter) this.CategoryFilter = [];
      this.CategoryFilter.push(selectedCategory);
    } else {
      this.CategoryFilter = this.CategoryFilter?.filter(category => category !== selectedCategory) || null;
    }
    this.getPaginatedProducts({});
  }

  onPriceRangeChange() {
    this.MinPriceRangeFilter = this.minPriceRangeFilter.toString();
    this.MaxPriceRangeFilter = this.maxPriceRangeFilter.toString();
    this.getPaginatedProducts({});
  }

  resetFilters() {
    this.BrandFilter = null;
    this.CategoryFilter = null;
    this.minPriceRangeFilter = this.priceSliderOptions.floor || 0;
    this.maxPriceRangeFilter = this.priceSliderOptions.ceil || 10000;
    this.MinPriceRangeFilter = null;
    this.MaxPriceRangeFilter = null;
    this.getPaginatedProducts({});
  }

  get totalPagesArray(): number[] {
    const totalPages = Math.ceil(this.totalRecords / this.pageSize);
    return Array.from({ length: totalPages }, (_, i) => i + 1);
  }

  pageChange(page: number): void {
    this.currentPage = page;
    this.getPaginatedProducts({});
  }

  addProduct() {
    this.loading = true;
    this.productsAPIService.InsertProduct(this.cardData).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Product inserted successfully");
          this.getPaginatedProducts({});
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  addNewProduct(): void {
    const dialogRef = this.dialog.open(EditProductDialogComponent, {
      width: '900px',
      data: this.cardData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cardData = result;
        this.addProduct();
      }
    });
  }
}
