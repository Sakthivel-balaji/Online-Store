using ModelService;

namespace LogicService
{
    public interface IProductsLogic
    {
        Task<Dictionary<string,List<string>>> GetDropdownValues();
        Task<PaginationResponse<List<ProductsModel>>> GetAll(PaginationRequest paginationRequest);
        Task<ResponseModel> InsertProduct(ProductModel product);
        Task<ProductModel> GetByProductId(int id);
        Task<ResponseModel> UpdateProduct(ProductModel product);
        Task<ResponseModel> DeleteProduct(int id);
    }
}
