export interface ReviewsModel {
    productId?: number;
    averageRating: number;
    reviews?: ReviewInfoModel[];
}

export interface ReviewInfoModel {
    reviewId: number;
    customerId?: number;
    userName?: string;
    rating?: number;
    comment?: string;
    createdAt?: Date;
}

export interface ReviewInsertModel {
    customerId: number;
    productId: number;
    rating: number;
    comment?: string;
}

