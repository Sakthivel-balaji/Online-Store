using ModelService;

namespace LogicService
{
    public interface ICustomersLogic
    {
        Task<Dictionary<string,List<string>>> GetDropdownValues();
        Task<PaginationResponse<List<CustomersModel>>> GetAll(PaginationRequest paginationRequest);
        Task<CustomerModel> GetByCustomerId(int id);
        Task<ResponseModel> UpdateCustomer(CustomerUpdateModel customer);
        Task<ResponseModel> DeleteCustomer(int id);
    }
}
