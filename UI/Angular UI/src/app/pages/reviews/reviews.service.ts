import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginationRequest } from '../../shared/models/pagination.model';
import { ReviewInsertModel } from '../../shared/models/reviews.model';

@Injectable({
  providedIn: 'root'
})

export class ReviewsService {
  constructor(
    private httpClient: HttpClient
  ) { }

  token: string = "Bearer " + localStorage.getItem("token");
  OnlineStoreWebAPI = environment.OnlineStoreWebAPI + environment.Reviews;

  InsertReview(review: ReviewInsertModel): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.post(this.OnlineStoreWebAPI, review, { headers: headers });
  }

  GetByProductId(page: PaginationRequest, productId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });

    let params = new HttpParams()
      .set('productId', productId.toString())
      .set('PageNumber', page.pageNumber.toString())
      .set('PageSize', page.pageSize.toString())
      .set('SortColumn', page.sortColumn ?? "")
      .set('SortOrder', page.sortOrder ?? "")
      .set('FilterColumn', page.filterColumn ?? "")
      .set('FilterValue', page.filterValue ?? "");

    return this.httpClient.get(`${this.OnlineStoreWebAPI}${productId}`, { headers, params });
  }

  DeleteReview(reviewId: number): Observable<any> {
    var headers = new HttpHeaders({ 'Authorization': this.token, 'Content-Type': 'application/json; charset=utf-8' });
    return this.httpClient.delete(this.OnlineStoreWebAPI + reviewId, { headers: headers });
  }
}
