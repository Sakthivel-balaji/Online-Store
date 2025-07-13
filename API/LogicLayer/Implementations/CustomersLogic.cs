using DataService;
using ModelService;

namespace LogicService
{
    public class CustomersLogic : ICustomersLogic
    {
        private readonly ICustomersData _customerData;

        public CustomersLogic(ICustomersData customerData)
        {
            _customerData = customerData;
        }

        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            return await _customerData.GetDropdownValues();
        }

        public async Task<PaginationResponse<List<CustomersModel>>> GetAll(PaginationRequest paginationRequest)
        {
            return await _customerData.GetAll(paginationRequest);
        }

        public async Task<CustomerModel> GetByCustomerId(int id)
        {
            return await _customerData.GetByCustomerId(id);
        }

        public async Task<ResponseModel> UpdateCustomer(CustomerUpdateModel customer)
        {
            return await _customerData.UpdateCustomer(customer);
        }

        public async Task<ResponseModel> DeleteCustomer(int id)
        {
            return await _customerData.DeleteCustomer(id);
        }
    }
}