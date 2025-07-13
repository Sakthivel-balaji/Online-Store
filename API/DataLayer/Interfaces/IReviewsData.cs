using ModelService;

namespace DataService
{
    public interface IReviewsData
    {
        Task<ResponseModel> InsertReview(ReviewInsertModel review);
        Task<PaginationResponse<ReviewsModel>> GetByProductId(PaginationRequest page,int productId);
        Task<ResponseModel> DeleteReview(int id);
    }
}
