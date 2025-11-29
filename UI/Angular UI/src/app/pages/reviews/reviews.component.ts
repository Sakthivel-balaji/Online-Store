import { Component, Input, OnInit } from '@angular/core';
import { ReviewsService } from './reviews.service';
import { TokenDecodeService } from '../../shared/services/token-decode.service';
import { PopupmessageService } from '../../shared/services/popupmessage.service';
import { ReviewInsertModel, ReviewsModel } from '../../shared/models/reviews.model';

@Component({
  selector: 'app-reviews',
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.css']
})

export class ReviewsComponent implements OnInit {
  role: string = '';
  loading: boolean = false;
  data: ReviewsModel = {} as ReviewsModel;
  params: any;
  pageSize = 5;
  currentPage = 1;
  totalPages = 0;
  selectedRatingforFilter: string | null = null;
  token:any;

  rating: number = 0;
  customerId: number = 0;
  comment: string = '';
  @Input() productId: number = 0;

  constructor(
    private reviewsAPIService: ReviewsService,
    private tokenDecodeService: TokenDecodeService,
    private popupService: PopupmessageService,
  ) { }

  ngOnInit(): void {
    this.token = localStorage.getItem('token');
    this.role = this.tokenDecodeService.getUserRole();
    this.customerId = Number(this.tokenDecodeService.getCustomerId());
    if (this.productId != 0) {
      this.getPaginatedReviews();
    }
  }

  getPaginatedReviews() {
    this.loading = true;

    this.params = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'CreatedAt',
      sortOrder: 'DESC',
      filterColumn: this.selectedRatingforFilter ? 'Rating' : '',
      filterValue: this.selectedRatingforFilter || ''
    };

    this.reviewsAPIService.GetByProductId(this.params, Number(this.productId)).subscribe(
      res => {
        this.data = res.items;
        this.totalPages = Math.ceil(res.page.recordCount / this.pageSize);
        this.loading = false;
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong while fetching reviews");
      }
    );
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.getPaginatedReviews();
    }
  }

  onRatingFilterChange() {
    this.currentPage = 1;
    this.getPaginatedReviews();
  }

  insertReview() {
    if (this.rating === 0 || this.comment.trim() === '') {
      this.popupService.showErrorMessage("Please provide a rating and comment");
      return;
    }

    this.loading = true;

    const reviewPayload: ReviewInsertModel = {
      customerId: this.customerId,
      productId: this.productId,
      rating: this.rating,
      comment: this.comment
    };

    this.reviewsAPIService.InsertReview(reviewPayload).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Review submitted successfully");
          this.comment = '';
          this.rating = 0;
          this.getPaginatedReviews();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }

  deleteReview(reviewId: number) {
    this.loading = true;
    this.reviewsAPIService.DeleteReview(reviewId).subscribe(
      res => {
        this.loading = false;
        if (res.statusCode != 200) {
          this.popupService.showErrorMessage(res.message);
        }
        else {
          this.popupService.showSuccessMessage("Review deleted successfully");
          this.getPaginatedReviews();
        }
      },
      err => {
        this.loading = false;
        console.log(err);
        this.popupService.showErrorMessage("Something went wrong");
      }
    );
  }
}
