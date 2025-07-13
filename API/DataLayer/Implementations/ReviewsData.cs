using Microsoft.EntityFrameworkCore;
using ModelService;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class ReviewsData : IReviewsData
    {
        private readonly OnlineStoreDbContext _dbContext;
        public ReviewsData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseModel> InsertReview(ReviewInsertModel review)
        {
            try
            {
                if (review == null || review.CustomerId <= 0 || review.ProductId <= 0 || review.Rating < 1 || review.Rating > 5 || string.IsNullOrWhiteSpace(review.Comment))
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = "Invalid review data. Please provide valid details."
                    };
                }

                var newReview = new Review
                {
                    CustomerId = review.CustomerId,
                    ProductId = review.ProductId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Reviews.AddAsync(newReview);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Review inserted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while inserting the Review: {ex.Message}"
                };
            }
        }

        public async Task<PaginationResponse<ReviewsModel>> GetByProductId(PaginationRequest page, int productId)
        {
            try
            {
                var query = _dbContext.Reviews
                    .Where(r => r.ProductId == productId && r.IsDeleted == false);

                if (!string.IsNullOrEmpty(page.FilterColumn) && page.FilterColumn.ToLower() == "rating" && !string.IsNullOrEmpty(page.FilterValue))
                {
                    if (int.TryParse(page.FilterValue, out int ratingValue) && ratingValue >= 1 && ratingValue <= 5)
                    {
                        query = query.Where(r => r.Rating == ratingValue);
                    }
                }

                int totalRecords = await query.CountAsync();

                if (!string.IsNullOrEmpty(page.SortColumn))
                {
                    bool asc = page.SortOrder?.ToLower() != "desc";

                    query = asc
                        ? query.OrderBy(r => EF.Property<object>(r, page.SortColumn))
                        : query.OrderByDescending(r => EF.Property<object>(r, page.SortColumn));
                }
                else
                {
                    query = query.OrderByDescending(r => r.CreatedAt);
                }

                var reviewsList = await query
                    .Skip((page.PageNumber - 1) * page.PageSize)
                    .Take(page.PageSize)
                    .Select(r => new ReviewInfoModel
                    {
                        ReviewId = r.ReviewId,
                        CustomerId = r.CustomerId,
                        UserName = r.Customer.UserName,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                var averageRating = await _dbContext.Reviews
                    .Where(r => r.ProductId == productId && r.IsDeleted == false)
                    .AverageAsync(r => (decimal?)r.Rating) ?? 0;

                var reviewsModel = new ReviewsModel
                {
                    ProductId = productId,
                    AverageRating = Math.Round(averageRating, 1),
                    Reviews = reviewsList
                };

                var response = new PaginationResponse<ReviewsModel>(
                    items: reviewsModel,
                    page: new PaginationDetails
                    {
                        RecordCount = totalRecords,
                        Page = page.PageNumber,
                        Size = page.PageSize,
                        PageCount = (int)Math.Ceiling((double)totalRecords / page.PageSize)
                    },
                    sort: new SortDetails
                    {
                        By = page.SortColumn,
                        Order = page.SortOrder
                    },
                    filter: new FilterDetails
                    {
                        By = page.FilterColumn,
                        Query = page.FilterValue
                    }
                );

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong..." + ex.Message);
            }
        }

        public async Task<ResponseModel> DeleteReview(int id)
        {
            try
            {
                var existingReview = await _dbContext.Reviews
                    .FirstOrDefaultAsync(c => c.ReviewId == id);

                if (existingReview == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Review not found."
                    };
                }

                existingReview.IsDeleted = true;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Review updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the Review: {ex.Message}"
                };
            }
        }
    }
}
