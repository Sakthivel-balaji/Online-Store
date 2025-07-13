using ModelService;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class OrdersData : IOrdersData
    {
        private readonly OnlineStoreDbContext _context;
        public OrdersData(OnlineStoreDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            try
            {
                var dropdowns = new Dictionary<string, List<string>>
                {
                    ["OrderIds"] = await _context.Orders
                        .Where(o => o.IsDeleted == false)
                        .OrderBy(o => o.OrderId)
                        .Select(o => o.OrderId.ToString())
                        .Distinct()
                        .ToListAsync(),

                    ["CustomerIds"] = await _context.Customers
                        .Where(c => c.IsDeleted == false)
                        .OrderBy(c => c.CustomerId)
                        .Select(c => c.CustomerId.ToString())
                        .Distinct()
                        .ToListAsync(),

                    ["Emails"] = await _context.Customers
                        .Where(c => c.IsDeleted == false && c.IsDeleted == false && !string.IsNullOrEmpty(c.Email))
                        .OrderBy(c => c.Email)
                        .Select(c => c.Email)
                        .Distinct()
                        .ToListAsync()
                };

                return dropdowns;
            }
            catch
            {
                return new Dictionary<string, List<string>>();
            }
        }

        public async Task<PaginationResponse<List<OrdersModel>>> GetOrders(PaginationRequest request)
        {
            try
            {
                var query = _context.Orders
                    .Where(o => o.IsDeleted == false)
                    .Include(o => o.Customer)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.FilterValue))
                {
                    var filters = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(request.FilterValue);
                    if (filters != null)
                    {
                        if (!string.IsNullOrEmpty(filters["CustomerId"]))
                        {
                            var cid = int.Parse(filters["CustomerId"]);
                            query = query.Where(o => o.CustomerId == cid);
                        }
                        if (!string.IsNullOrEmpty(filters["OrderId"]))
                        {
                            var oid = int.Parse(filters["OrderId"]);
                            query = query.Where(o => o.OrderId == oid);
                        }
                        if (!string.IsNullOrEmpty(filters["Email"]))
                        {
                            query = query.Where(o => o.Customer.Email.Contains(filters["Email"]));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    bool asc = request.SortOrder?.ToLower() != "desc";
                    query = asc
                        ? query.OrderBy(o => EF.Property<object>(o, request.SortColumn))
                        : query.OrderByDescending(o => EF.Property<object>(o, request.SortColumn));
                }
                else
                {
                    query = query.OrderBy(o => o.OrderDate);
                }

                int count = await query.CountAsync();

                var orders = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(o => new OrdersModel
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        OrderDate = o.OrderDate,
                        DeliveryDate = o.DeliveryDate,
                        Status = o.Status,
                        Email = o.Customer.Email,
                        TotalPrice = o.TotalPrice
                    })
                    .ToListAsync();

                return new PaginationResponse<List<OrdersModel>>(
                    orders,
                    new PaginationDetails
                    {
                        Page = request.PageNumber,
                        Size = request.PageSize,
                        RecordCount = count,
                        PageCount = (int)Math.Ceiling(count / (double)request.PageSize)
                    },
                    new SortDetails { By = request.SortColumn, Order = request.SortOrder },
                    new FilterDetails { By = request.FilterColumn, Query = request.FilterValue }
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong " + ex.Message);
            }
        }

        public async Task<ResponseModel> InsertOrder(OrderInsertModel order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.CustomerId == order.CustomerId && ci.IsDeleted == false)
                    .ToListAsync();

                if (cartItems == null || !cartItems.Any())
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = "Cart is empty. Please add items before placing an order."
                    };
                }

                var stockErrors = new List<string>();
                decimal totalOrderPrice = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in cartItems)
                {
                    var product = item.Product;
                    if (product == null || product.StockQuantity < item.Quantity)
                    {
                        stockErrors.Add(product?.Name ?? $"Product ID {item.ProductId}");
                        continue;
                    }

                    var discountAmount = product.Discount.HasValue && product.Discount.Value > 0
                        ? product.Price * (product.Discount.Value / 100)
                        : 0;

                    var finalUnitPrice = product.Price - discountAmount;
                    totalOrderPrice += finalUnitPrice * item.Quantity;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = finalUnitPrice,
                        IsDeleted = false
                    });
                }

                if (stockErrors.Any())
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = $"Insufficient stock for: {string.Join(", ", stockErrors)}"
                    };
                }

                var newOrder = new Order
                {
                    CustomerId = order.CustomerId,
                    AddressId = order.AddressId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = totalOrderPrice,
                    IsDeleted = false,
                    Status = "Placed"
                };

                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                foreach (var orderItem in orderItems)
                {
                    orderItem.OrderId = newOrder.OrderId;
                    await _context.OrderItems.AddAsync(orderItem);

                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= orderItem.Quantity;
                    }
                }

                foreach (var cartItem in cartItems)
                {
                    cartItem.IsDeleted = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Order placed successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"Error placing order: {ex.Message}"
                };
            }
        }

        public async Task<OrderModel> GetByOrderId(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Address)
                    .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.IsDeleted == false);

                if (order == null) throw new Exception("Order Not Found");

                return new OrderModel
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    Email = order.Customer.Email,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    DeliveryInfo = new DeliveryModel
                    {
                        AddressId = order.Address?.AddressId ?? 0,
                        Address = order.Address?.Address,
                        City = order.Address?.City,
                        State = order.Address?.State,
                        Country = order.Address?.Country,
                        Phone = order.Address?.Phone,
                        PostalCode = order.Address?.PostalCode
                    },
                    Products = order.OrderItems
                        .Where(i => i.IsDeleted == false)
                        .Select(i => new ProductInfoModel
                        {
                            ProductId = i.ProductId,
                            Name = i.Product.Name,
                            UnitPrice = i.UnitPrice,
                            Quantity = i.Quantity,
                            Category = i.Product.Category,
                            Brand = i.Product.Brand,
                            Image = i.Product.Image
                        }).ToList()
                };
            }
            catch
            {
                throw new Exception("Order Id is Invalid");
            }
        }

        public async Task<ResponseModel> UpdateOrder(OrderUpdateModel model)
        {
            try
            {
                var order = await _context.Orders.FindAsync(model.OrderId);

                if (order == null || order.IsDeleted == true)
                {
                    return new ResponseModel { StatusCode = 404, Message = "Order not found" };
                }

                order.Status = model.Status;
                order.DeliveryDate = model.DeliveryDate;

                await _context.SaveChangesAsync();

                return new ResponseModel { StatusCode = 200, Message = "Order updated" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { StatusCode = 500, Message = $"Update failed: {ex.Message}" };
            }
        }

        public async Task<ResponseModel> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.IsDeleted == false);

                if (order == null)
                {
                    return new ResponseModel { StatusCode = 404, Message = "Order not found" };
                }

                order.Status = "Cancelled";
                order.IsDeleted = true;

                foreach (var item in order.OrderItems)
                {
                    item.IsDeleted = true;
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                        product.StockQuantity += item.Quantity;
                }

                await _context.SaveChangesAsync();

                return new ResponseModel { StatusCode = 200, Message = "Order deleted" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { StatusCode = 500, Message = $"Deletion failed: {ex.Message}" };
            }
        }
    }
}


