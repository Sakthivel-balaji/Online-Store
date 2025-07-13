using DataService;
using ModelService;

namespace LogicService
{
    public class OrdersLogic : IOrdersLogic
    {
        private readonly IOrdersData _ordersData;
        public OrdersLogic(IOrdersData ordersData)
        {
            _ordersData = ordersData;
        }

        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            return await _ordersData.GetDropdownValues();
        }

        public async Task<PaginationResponse<List<OrdersModel>>> GetOrders(PaginationRequest paginationRequest)
        {
            return await _ordersData.GetOrders(paginationRequest);
        }

        public async Task<ResponseModel> InsertOrder(OrderInsertModel order)
        {
            return await _ordersData.InsertOrder(order);
        }

        public async Task<OrderModel> GetByOrderId(int id)
        {
            return await _ordersData.GetByOrderId(id);
        }

        public async Task<ResponseModel> UpdateOrder(OrderUpdateModel order)
        {
            return await _ordersData.UpdateOrder(order);
        }

        public async Task<ResponseModel> DeleteOrder(int id)
        {
            return await _ordersData.DeleteOrder(id);
        }
    }
}