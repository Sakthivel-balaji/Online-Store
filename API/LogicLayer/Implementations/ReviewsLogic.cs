using DataService;
using ModelService;

namespace LogicService
{
    public class ReviewsLogic : IReviewsLogic
    {
        private readonly IReviewsData _reviewsData;
        public ReviewsLogic(IReviewsData reviewsData)
        {
            _reviewsData = reviewsData;
        }

        public async Task<ResponseModel> InsertReview(ReviewInsertModel review)
        {
            return await _reviewsData.InsertReview(review);
        }

        public async Task<PaginationResponse<ReviewsModel>> GetByProductId(PaginationRequest page,int productId)
        {
            return await _reviewsData.GetByProductId(page,productId);
        }

        public async Task<ResponseModel> DeleteReview(int id)
        {
            return await _reviewsData.DeleteReview(id);
        }
    }
}