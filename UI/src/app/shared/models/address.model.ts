export interface AddressModel {
    addressId?: number;
    customerId?: number;
    address?: string;
    city?: string;
    state?: string;
    country?: string;
    postalCode?: string;
    phone?: number;
    createdAt?: Date;
    isDeleted?: boolean;
    isPrimary?: boolean;
}
