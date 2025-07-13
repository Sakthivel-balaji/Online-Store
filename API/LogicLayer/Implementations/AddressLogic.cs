using DataService;
using ModelService;

namespace LogicService
{
    public class AddressLogic : IAddressLogic
    {
        private readonly IAddressData _addressData;
        public AddressLogic(IAddressData addressData)
        {
            _addressData = addressData;
        }

        public async Task<ResponseModel> InsertAddress(AddressModel address)
        {
            return await _addressData.InsertAddress(address);
        }

        public async Task<List<AddressModel>> GetByCustomerId(int id)
        {
            return await _addressData.GetByCustomerId(id);
        }

        public async Task<ResponseModel> UpdateAddress(AddressModel address)
        {
            return await _addressData.UpdateAddress(address);
        }

        public async Task<ResponseModel> DeleteAddress(int id)
        {
            return await _addressData.DeleteAddress(id);
        }
    }
}