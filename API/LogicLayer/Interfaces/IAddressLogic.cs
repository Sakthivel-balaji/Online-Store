using ModelService;

namespace LogicService
{
    public interface IAddressLogic
    {
        Task<ResponseModel> InsertAddress(AddressModel address);
        Task<List<AddressModel>> GetByCustomerId(int id);
        Task<ResponseModel> UpdateAddress(AddressModel address);
        Task<ResponseModel> DeleteAddress(int id);
    }
}
