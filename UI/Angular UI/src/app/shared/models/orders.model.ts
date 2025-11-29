export interface OrderModel {
    orderId?: number;
    customerId: number;
    orderDate?: Date;
    totalPrice?: number;
    deliveryDate?: Date;
    status?: string;
    email?: string;
    deliveryInfo?: DeliveryModel;
    products?: ProductInfoModel[];
}

export interface OrdersModel {
    orderId: number;
    customerId: number;
    orderDate?: Date;
    deliveryDate?: Date;
    status?: string;
    email?: string;
    totalPrice?: number;
}

export interface OrderInsertModel {
    customerId: number;
    addressId: number;
}

export interface OrderUpdateModel {
    orderId: number;
    status?: string;
    deliveryDate?: Date;
}

export interface ProductInfoModel {
    productId: number;
    name?: string;
    unitPrice: number;
    quantity: number;
    category?: string;
    brand?: string;
    image?: string;
}

export interface DeliveryModel {
    addressId: number;
    address?: string;
    city?: string;
    state?: string;
    country?: string;
    postalCode?: string;
    phone?: string;
}
