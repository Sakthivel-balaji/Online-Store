using DataService;
using ModelService;

namespace LogicService
{
    public class ProductsLogic : IProductsLogic
    {
        private readonly IProductsData _productsData;
        public ProductsLogic(IProductsData productsData)
        {
            _productsData = productsData;
        }

        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            return await _productsData.GetDropdownValues();
        }

        public async Task<PaginationResponse<List<ProductsModel>>> GetAll(PaginationRequest paginationRequest)
        {
            return await _productsData.GetAll(paginationRequest);
        }

        public async Task<ResponseModel> InsertProduct(ProductModel product)
        {
            return await _productsData.InsertProduct(product);
        }

        public async Task<ProductModel> GetByProductId(int id)
        {
            return await _productsData.GetByProductId(id);
        }

        public async Task<ResponseModel> UpdateProduct(ProductModel product)
        {
            return await _productsData.UpdateProduct(product);
        }

        public async Task<ResponseModel> DeleteProduct(int id)
        {
            return await _productsData.DeleteProduct(id);
        }
    }
}