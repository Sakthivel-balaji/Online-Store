using ModelService;

namespace DataService
{
    public interface IAddressData
    {
        Task<ResponseModel> InsertAddress(AddressModel address);
        Task<List<AddressModel>> GetByCustomerId(int id);
        Task<ResponseModel> UpdateAddress(AddressModel address);
        Task<ResponseModel> DeleteAddress(int id);
    }
}
