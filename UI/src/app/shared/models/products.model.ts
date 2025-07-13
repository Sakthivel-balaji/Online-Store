export interface ProductModel {
    productId: number;
    name: string;
    description?: string;
    category?: string;
    brand?: string;
    price: number;
    discount?: number;
    stockQuantity: number;
    averageRating?: number;
    image?: string;
    weight?: number;
    length?: number;
    breadth?: number;
    height?: number;
    isFeatured?: boolean;
    isPopular?: boolean;
    createdAt?: Date;
    isDeleted?: boolean;
}

export interface ProductsModel {
    productId: number;
    name?: string;
    category?: string;
    brand?: string;
    price: number;
    discount?: number;
    averageRating?: number;
    image?: string;
    stockQuantity: number;
    isFeatured?: boolean;
    isPopular?: boolean;
}
