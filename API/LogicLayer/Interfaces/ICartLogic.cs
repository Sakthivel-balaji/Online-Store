using ModelService;

namespace LogicService
{
    public interface ICartLogic
    {
        Task<ResponseModel> InsertItem(CartInsertModel cart);
        Task<CartModel> GetByCustomerId(int custId);
        Task<ResponseModel> UpdateItem(CartUpdateModel cart);
        Task<ResponseModel> DeleteItem(int cartId);
    }
}
