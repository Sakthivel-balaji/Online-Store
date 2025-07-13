using DataService;
using ModelService;

namespace LogicService
{
    public class CartLogic : ICartLogic
    {
        private readonly ICartData _cartData;
        public CartLogic(ICartData cartData)
        {
            _cartData = cartData;
        }

        public async Task<ResponseModel> InsertItem(CartInsertModel cart)
        {
            return await _cartData.InsertItem(cart);
        }

        public async Task<CartModel> GetByCustomerId(int custId)
        {
            return await _cartData.GetByCustomerId(custId);
        }

        public async Task<ResponseModel> UpdateItem(CartUpdateModel cart)
        {
            return await _cartData.UpdateItem(cart);
        }

        public async Task<ResponseModel> DeleteItem(int cartId)
        {
            return await _cartData.DeleteItem(cartId);
        }
    }
}