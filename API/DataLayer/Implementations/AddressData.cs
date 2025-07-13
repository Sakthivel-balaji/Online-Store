using Microsoft.EntityFrameworkCore;
using ModelService;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class AddressData : IAddressData
    {
        private readonly OnlineStoreDbContext _dbContext;
        public AddressData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<AddressModel>> GetByCustomerId(int id)
        {
            try
            {
                var address = await _dbContext.DeliveryAddresses
                    .Where(c => c.CustomerId == id && c.IsDeleted == false)
                    .Select(c => new AddressModel
                    {
                        AddressId = c.AddressId,
                        CustomerId = c.CustomerId,
                        Phone = c.Phone,
                        Address = c.Address,
                        City = c.City,
                        State = c.State,
                        Country = c.Country,
                        PostalCode = c.PostalCode,
                        CreatedAt = c.CreatedAt,
                        IsDeleted = c.IsDeleted,
                        IsPrimary = c.IsPrimary
                    })
                    .ToListAsync();

                if (address == null)
                    throw new Exception("Customer not found");

                return address;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong..." + ex.Message);
            }
        }

        public async Task<ResponseModel> InsertAddress(AddressModel address)
        {
            try
            {
                bool addressExists = await _dbContext.DeliveryAddresses
                    .AnyAsync(a => a.CustomerId == address.CustomerId && a.IsDeleted == false);

                var newAddress = new DeliveryAddress
                {
                    CustomerId = address.CustomerId,
                    Phone = address.Phone ?? "",
                    Address = address.Address ?? "",
                    City = address.City ?? "",
                    State = address.State ?? "",
                    Country = address.Country ?? "",
                    PostalCode = address.PostalCode ?? "",
                    CreatedAt = DateTime.UtcNow,
                    IsPrimary = !addressExists
                };

                await _dbContext.DeliveryAddresses.AddAsync(newAddress);
                await _dbContext.SaveChangesAsync();

                if (newAddress.IsPrimary)
                {
                    var otherAddresses = await _dbContext.DeliveryAddresses
                        .Where(a => a.CustomerId == address.CustomerId && a.AddressId != newAddress.AddressId && a.IsDeleted == false)
                        .ToListAsync();

                    foreach (var otherAddress in otherAddresses)
                    {
                        otherAddress.IsPrimary = false;
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Address is inserted successfully."
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while inserting the Address: {ex.Message}");
            }
        }

        public async Task<ResponseModel> UpdateAddress(AddressModel address)
        {
            try
            {
                var existingAddress = await _dbContext.DeliveryAddresses
                    .FirstOrDefaultAsync(c => c.AddressId == address.AddressId);

                if (existingAddress == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Address not found."
                    };
                }

                existingAddress.Address = address.Address ?? "";
                existingAddress.City = address.City ?? "";
                existingAddress.State = address.State ?? "";
                existingAddress.Country = address.Country ?? "";
                existingAddress.PostalCode = address.PostalCode ?? "";
                existingAddress.Phone = address.Phone ?? "";
                existingAddress.IsPrimary = address.IsPrimary;

                await _dbContext.SaveChangesAsync();

                if (address.IsPrimary)
                {
                    var otherAddresses = await _dbContext.DeliveryAddresses
                        .Where(a => a.CustomerId == existingAddress.CustomerId && a.AddressId != existingAddress.AddressId && a.IsDeleted == false)
                        .ToListAsync();

                    foreach (var otherAddress in otherAddresses)
                    {
                        otherAddress.IsPrimary = false;
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Address updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the Address: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel> DeleteAddress(int id)
        {
            try
            {
                var address = await _dbContext.DeliveryAddresses
                    .FirstOrDefaultAsync(c => c.AddressId == id);

                if (address == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Address not found."
                    };
                }

                bool wasPrimary = address.IsPrimary;
                address.IsDeleted = true;
                address.IsPrimary = false;
                await _dbContext.SaveChangesAsync();

                if (wasPrimary)
                {
                    var anotherAddress = await _dbContext.DeliveryAddresses
                        .Where(a => a.CustomerId == address.CustomerId && a.IsDeleted == false)
                        .FirstOrDefaultAsync();

                    if (anotherAddress != null)
                    {
                        anotherAddress.IsPrimary = true;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Address deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while deleting the Address: {ex.Message}"
                };
            }
        }
    }
}
