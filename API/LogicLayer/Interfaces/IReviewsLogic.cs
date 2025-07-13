using ModelService;

namespace LogicService
{
    public interface IReviewsLogic
    {
        Task<ResponseModel> InsertReview(ReviewInsertModel review);
        Task<PaginationResponse<ReviewsModel>> GetByProductId(PaginationRequest page,int productId);
        Task<ResponseModel> DeleteReview(int id);
    }
}
