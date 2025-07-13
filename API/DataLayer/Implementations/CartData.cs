using Microsoft.EntityFrameworkCore;
using ModelService;
using OnlineStoreWebAPI.DataContext;

namespace DataService
{
    public class CartData : ICartData
    {
        private readonly OnlineStoreDbContext _dbContext;
        public CartData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> InsertItem(CartInsertModel cart)
        {
            try
            {
                if (cart == null || cart.Quantity <= 0)
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = "Quantity must be greater than 0."
                    };
                }

                var existingItem = await _dbContext.CartItems
                    .FirstOrDefaultAsync(c => c.CustomerId == cart.CustomerId && c.ProductId == cart.ProductId && c.IsDeleted == false);

                if (existingItem != null)
                {
                    existingItem.Quantity += cart.Quantity;
                }
                else
                {
                    var newCart = new CartItem
                    {
                        CustomerId = cart.CustomerId,
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        IsDeleted = false
                    };

                    await _dbContext.CartItems.AddAsync(newCart);
                }

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Item inserted to cart successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while inserting into the cart: {ex.Message}"
                };
            }
        }

        public async Task<CartModel> GetByCustomerId(int custId)
        {
            try
            {
                var cartItems = await _dbContext.CartItems
                .Where(ci => ci.CustomerId == custId && ci.IsDeleted == false)
                .Select(ci => new
                {
                    ci.CartItemId,
                    ci.ProductId,
                    ci.Product.Image,
                    ci.Product.Name,
                    ci.Product.Category,
                    ci.Product.Brand,
                    ci.Product.Discount,
                    ci.Product.StockQuantity,
                    ci.Quantity,
                    ci.Product.Price,
                })
                .ToListAsync();

                var products = cartItems.Select(ci =>
                {
                    var priceBeforeDiscount = ci.Price * ci.Quantity;
                    var discountAmount = (ci.Discount.HasValue && ci.Discount.Value > 0)
                        ? (ci.Price * (ci.Discount.Value / 100)) * ci.Quantity
                        : 0;

                    var priceAfterDiscount = priceBeforeDiscount - discountAmount;

                    return new CartProductsModel
                    {
                        CartItemId = ci.CartItemId,
                        ProductId = ci.ProductId,
                        ProductImage = ci.Image,
                        ProductName = ci.Name,
                        Quantity = ci.Quantity,
                        Category = ci.Category,
                        Brand = ci.Brand,
                        StockQuantity = ci.StockQuantity,
                        Discount = ci.Discount,
                        PriceBeforeDiscount = Math.Round(priceBeforeDiscount, 2),
                        PriceAfterDiscount = Math.Round(priceAfterDiscount, 2)
                    };
                }).ToList();

                var itemSubTotal = products.Sum(p => p.PriceBeforeDiscount ?? 0);
                var subTotal = products.Sum(p => p.PriceAfterDiscount ?? 0);
                var discountTotal = itemSubTotal - subTotal;

                return new CartModel
                {
                    CustomerId = custId,
                    Products = products,
                    ItemSubTotal = Math.Round(itemSubTotal, 2),
                    TotalDiscountPrice = Math.Round(discountTotal, 2),
                    SubTotal = Math.Round(subTotal, 2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong..." + ex.Message);
            }
        }

        public async Task<ResponseModel> UpdateItem(CartUpdateModel cart)
        {
            try
            {
                if (cart.Quantity <= 0)
                {
                    return new ResponseModel
                    {
                        StatusCode = 400,
                        Message = "Quantity must be greater than 0."
                    };
                }

                var existingCartItem = await _dbContext.CartItems
                    .FirstOrDefaultAsync(c => c.CartItemId == cart.CartItemId && c.IsDeleted == false);

                if (existingCartItem == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Cart Item not found."
                    };
                }

                existingCartItem.Quantity = cart.Quantity;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Cart updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the Cart: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel> DeleteItem(int cartId)
        {
            try
            {
                var existingCartItem = await _dbContext.CartItems
                    .FirstOrDefaultAsync(c => c.CartItemId == cartId);

                if (existingCartItem == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Cart Item not found."
                    };
                }

                existingCartItem.IsDeleted = true;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Cart Item updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the Cart Item: {ex.Message}"
                };
            }
        }
    }
}
