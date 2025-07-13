export interface CartModel {
    customerId: number;
    itemSubTotal?: number;
    totalDiscountPrice?: number;
    subTotal?: number;
    products?: CartProductsModel[];
}

export interface CartProductsModel {
    cartItemId: number;
    productId: number;
    productImage?: string;
    productName?: string;
    quantity: number;
    category?: string;
    brand?: string;
    discount?: number;
    stockQuantity?: number;
    priceBeforeDiscount?: number;
    priceAfterDiscount?: number;
}

export interface CartUpdateModel {
    cartItemId: number;
    quantity: number;
}

export interface CartInsertModel {
    customerId: number;
    productId: number;
    quantity: number;
}

