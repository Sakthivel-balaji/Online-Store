using ModelService;

namespace DataService
{
    public interface IOrdersData
    {
        Task<Dictionary<string,List<string>>> GetDropdownValues();
        Task<PaginationResponse<List<OrdersModel>>> GetOrders(PaginationRequest paginationRequest);
        Task<ResponseModel> InsertOrder(OrderInsertModel order);
        Task<OrderModel> GetByOrderId(int id);
        Task<ResponseModel> UpdateOrder(OrderUpdateModel order);
        Task<ResponseModel> DeleteOrder(int id);
    }
}
