using ModelService;

namespace DataService
{
    public interface IProductsData
    {
        Task<Dictionary<string,List<string>>> GetDropdownValues();
        Task<PaginationResponse<List<ProductsModel>>> GetAll(PaginationRequest paginationRequest);
        Task<ResponseModel> InsertProduct(ProductModel product);
        Task<ProductModel> GetByProductId(int id);
        Task<ResponseModel> UpdateProduct(ProductModel product);
        Task<ResponseModel> DeleteProduct(int id);
    }
}
