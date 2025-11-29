import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductModel } from '../../../shared/models/products.model';

@Component({
  selector: 'app-edit-product-dialog',
  templateUrl: './edit-product-dialog.component.html',
  styleUrls: ['./edit-product-dialog.component.css']
})

export class EditProductDialogComponent implements OnInit {
  productForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditProductDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductModel,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.productForm = this.fb.group({
      productId: [this.data.productId || 0],
      name: [this.data.name || '', Validators.required],
      category: [this.data.category || '', Validators.required],
      brand: [this.data.brand || '', Validators.required],
      price: [this.data.price || null, [Validators.required, Validators.min(0)]],
      discount: [this.data.discount || null, [Validators.min(0), Validators.max(100)]],
      stockQuantity: [this.data.stockQuantity || null, [Validators.required, Validators.min(0)]],
      weight: [this.data.weight || null, [Validators.min(0)]],
      length: [this.data.length || null, [Validators.min(0)]],
      breadth: [this.data.breadth || null, [Validators.min(0)]],
      height: [this.data.height || null, [Validators.min(0)]],
      description: [this.data.description || '', Validators.required],
      isFeatured: [this.data.isFeatured || false],
      isPopular: [this.data.isPopular || false],
      image: [this.data.image || null]
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.productForm.valid) {
      this.dialogRef.close(this.productForm.value);
    }
  }

  onImageSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        const base64String = (reader.result as string).split(',')[1];
        this.productForm.patchValue({ image: base64String });
      };
      reader.readAsDataURL(file);
    }
  }
}
