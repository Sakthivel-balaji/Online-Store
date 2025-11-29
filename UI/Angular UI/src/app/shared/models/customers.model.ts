export interface CustomersModel {
    customerId?: number;
    email?: string;
    fullName?: string;
    phone?: string;
}

export interface CustomerModel {
    customerId?: number;
    email?: string;
    profilePicture?: string;
    fullName?: string;
    phone?: string;
    createdAt?: Date;
}

export interface CustomerUpdateModel {
    customerId?: number;
    profilePicture?: string;
    fullName?: string;
    phone?: string;
}

