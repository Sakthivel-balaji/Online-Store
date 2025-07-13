using Microsoft.EntityFrameworkCore;
using ModelService;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class CustomersData : ICustomersData
    {
        private readonly OnlineStoreDbContext _dbContext;

        public CustomersData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            var dropdownValues = new Dictionary<string, List<string>>();

            var CustomerIds = await _dbContext.Customers
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.CustomerId)
                .Select(c => c.CustomerId.ToString())
                .Distinct()
                .ToListAsync();

            var CustomerNames = await _dbContext.Customers
                .Where(c => c.IsDeleted == false && !string.IsNullOrEmpty(c.FullName))
                .OrderBy(c => c.FullName)
                .Select(c => c.FullName ?? "")
                .Distinct()
                .ToListAsync();

            var CustomerEmails = await _dbContext.Customers
                .Where(c => c.IsDeleted == false && !string.IsNullOrEmpty(c.Email))
                .OrderBy(email => email)
                .Select(c => c.Email ?? "")
                .Distinct()
                .ToListAsync();

            dropdownValues.Add("CustomerIds", CustomerIds);
            dropdownValues.Add("CustomerNames", CustomerNames);
            dropdownValues.Add("CustomerEmails", CustomerEmails);

            return dropdownValues;
        }

        public async Task<PaginationResponse<List<CustomersModel>>> GetAll(PaginationRequest paginationRequest)
        {
            try
            {
                var filters = string.IsNullOrEmpty(paginationRequest.FilterValue)
                    ? new Dictionary<string, string>()
                    : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(paginationRequest.FilterValue);

                string customerIdFilter = filters?.GetValueOrDefault("CustomerId") ?? string.Empty;
                string fullNameFilter = filters?.GetValueOrDefault("FullName") ?? string.Empty;
                string emailFilter = filters?.GetValueOrDefault("Email") ?? string.Empty;

                var query = _dbContext.Customers.Where(c => c.IsDeleted == false);

                if (!string.IsNullOrEmpty(customerIdFilter))
                {
                    query = query.Where(cu => cu.CustomerId.ToString() == customerIdFilter);
                }

                if (!string.IsNullOrEmpty(fullNameFilter))
                {
                    query = query.Where(cu => cu.FullName == fullNameFilter);
                }

                if (!string.IsNullOrEmpty(emailFilter))
                {
                    query = query.Where(cu => cu.Email == emailFilter);
                }

                var totalRecords = await query.CountAsync();

                if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
                {
                    query = paginationRequest.SortOrder?.ToUpper() == "DESC"
                        ? query.OrderByDescending(c => EF.Property<object>(c, paginationRequest.SortColumn))
                        : query.OrderBy(c => EF.Property<object>(c, paginationRequest.SortColumn));
                }
                else
                {
                    query = query.OrderBy(c => c.CustomerId);
                }

                var paginatedResults = await query
                    .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                    .Take(paginationRequest.PageSize)
                    .Select(c => new CustomersModel
                    {
                        CustomerId = c.CustomerId,
                        Email = c.Email,
                        FullName = c.FullName,
                        Phone = c.Phone
                    })
                    .ToListAsync();

                var paginationDetails = new PaginationDetails
                {
                    RecordCount = totalRecords,
                    Page = paginationRequest.PageNumber,
                    Size = paginationRequest.PageSize,
                    PageCount = (int)Math.Ceiling(totalRecords / (double)paginationRequest.PageSize)
                };

                var sortDetails = new SortDetails
                {
                    By = paginationRequest.SortColumn,
                    Order = paginationRequest.SortOrder
                };

                return new PaginationResponse<List<CustomersModel>>(
                    paginatedResults,
                    paginationDetails,
                    sortDetails,
                    new FilterDetails { By = "Multiple", Query = paginationRequest.FilterValue }
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching customers: {ex.Message}");
            }
        }

        public async Task<CustomerModel> GetByCustomerId(int id)
        {
            try
            {
                var customer = await _dbContext.Customers
                    .Where(c => c.CustomerId == id && c.IsDeleted == false)
                    .Select(c => new CustomerModel
                    {
                        CustomerId = c.CustomerId,
                        Email = c.Email,
                        ProfilePicture = c.ProfilePicture,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (customer == null)
                    throw new Exception("Customer not found");

                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while fetching the customer: " + ex.Message);
            }
        }

        public async Task<ResponseModel> UpdateCustomer(CustomerUpdateModel customer)
        {
            try
            {
                var existingCustomer = await _dbContext.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId && c.IsDeleted == false);

                if (existingCustomer == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Customer not found."
                    };
                }

                if (string.IsNullOrEmpty(customer.FullName))
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = "Customer FullName is required."
                    };
                }

                existingCustomer.ProfilePicture = customer.ProfilePicture;
                existingCustomer.FullName = customer.FullName;
                existingCustomer.Phone = customer.Phone;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Customer updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the customer: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _dbContext.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id);

                if (customer == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Customer not found."
                    };
                }

                customer.IsDeleted = true;
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Customer deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while deleting the customer: {ex.Message}"
                };
            }
        }
    }
}
