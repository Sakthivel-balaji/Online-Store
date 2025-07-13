using ModelService;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DataContext;
using System.Text.Json;

namespace DataService
{
    public class ProductsData : IProductsData
    {
        private readonly OnlineStoreDbContext _dbContext;

        public ProductsData(OnlineStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, List<string>>> GetDropdownValues()
        {
            try
            {
                var categories = await _dbContext.Products
                    .Where(p => p.IsDeleted == false && !string.IsNullOrEmpty(p.Category))
                    .OrderBy(c => c.Category)
                    .Select(p => p.Category ?? "")
                    .Distinct()
                    .ToListAsync();

                var brands = await _dbContext.Products
                    .Where(p => p.IsDeleted == false && !string.IsNullOrEmpty(p.Brand))
                    .OrderBy(b => b.Brand)
                    .Select(p => p.Brand ?? "")
                    .Distinct()
                    .ToListAsync();

                var minPriceRange = await _dbContext.Products
                    .Where(p => p.IsDeleted == false)
                    .Select(p => p.Price)
                    .MinAsync();

                var maxPriceRange = await _dbContext.Products
                    .Where(p => p.IsDeleted == false)
                    .Select(p => p.Price)
                    .MaxAsync();

                return new Dictionary<string, List<string>>
                {
                    { "Categories", categories },
                    { "Brands", brands },
                    { "PriceRange", new List<string> { Math.Floor(minPriceRange).ToString(), Math.Floor(maxPriceRange).ToString() } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching dropdown values: {ex.Message}");
            }
        }

        public async Task<PaginationResponse<List<ProductsModel>>> GetAll(PaginationRequest paginationRequest)
        {
            try
            {
                var filter = string.IsNullOrEmpty(paginationRequest.FilterValue)
                    ? new ProductFilter()
                    : JsonSerializer.Deserialize<ProductFilter>(paginationRequest.FilterValue);

                var query = _dbContext.Products
                    .Where(p => p.IsDeleted == false)
                    .AsQueryable();

                if (filter?.Brands != null && filter.Brands.Any())
                {
                    query = query.Where(p => filter.Brands.Contains(p.Brand ?? ""));
                }

                if (filter?.Categories != null && filter.Categories.Any())
                {
                    query = query.Where(p => filter.Categories.Contains(p.Category ?? ""));
                }

                if (!string.IsNullOrEmpty(filter?.MinPriceRange) && decimal.TryParse(filter.MinPriceRange, out var minPrice))
                {
                    query = query.Where(p => p.Price >= minPrice);
                }

                if (!string.IsNullOrEmpty(filter?.MaxPriceRange) && decimal.TryParse(filter.MaxPriceRange, out var maxPrice))
                {
                    query = query.Where(p => p.Price <= maxPrice);
                }

                var totalRecords = await query.CountAsync();

                if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
                {
                    query = paginationRequest.SortOrder?.ToUpper() == "DESC"
                        ? query.OrderByDescending(p => EF.Property<object>(p, paginationRequest.SortColumn))
                        : query.OrderBy(p => EF.Property<object>(p, paginationRequest.SortColumn));
                }
                else
                {
                    query = query.OrderBy(p => p.ProductId);
                }

                var paginatedResults = await query
                    .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                    .Take(paginationRequest.PageSize)
                    .Select(p => new ProductsModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Category = p.Category,
                        Brand = p.Brand,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity,
                        Image = p.Image,
                        AverageRating = p.Reviews
                            .Where(r => r.IsDeleted == false)
                            .Average(r => (double?)r.Rating) ?? 0,
                        IsFeatured = p.IsFeatured,
                        IsPopular = p.IsPopular
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

                return new PaginationResponse<List<ProductsModel>>(
                    paginatedResults,
                    paginationDetails,
                    sortDetails,
                    new FilterDetails { By = "Multiple", Query = paginationRequest.FilterValue });
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while filtering products: {ex.Message}");
            }
        }

        public async Task<ProductModel> GetByProductId(int id)
        {
            try
            {
                var product = await _dbContext.Products
                    .Where(p => p.ProductId == id)
                    .Select(p => new ProductModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Category = p.Category,
                        Brand = p.Brand,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity,
                        Image = p.Image,
                        AverageRating = p.Reviews
                            .Where(r => r.IsDeleted == false)
                            .Average(r => (double?)r.Rating) ?? 0,
                        Weight = p.Weight,
                        Length = p.Length,
                        Breadth = p.Breadth,
                        Height = p.Height,
                        IsFeatured = p.IsFeatured,
                        IsPopular = p.IsPopular,
                        CreatedAt = p.CreatedAt,
                        IsDeleted = p.IsDeleted
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching the product: {ex.Message}");
            }
        }

        public async Task<ResponseModel> InsertProduct(ProductModel product)
        {
            try
            {
                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Category = product.Category,
                    Brand = product.Brand,
                    Price = product.Price,
                    Discount = product.Discount,
                    StockQuantity = product.StockQuantity,
                    Image = product.Image,
                    Weight = product.Weight,
                    Length = product.Length,
                    Breadth = product.Breadth,
                    Height = product.Height,
                    IsFeatured = product.IsFeatured,
                    IsPopular = product.IsPopular,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Products.AddAsync(newProduct);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 0,
                    Message = "Product inserted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while inserting the product: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel> UpdateProduct(ProductModel product)
        {
            try
            {
                var existingProduct = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                if (existingProduct == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Product not found."
                    };
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
                existingProduct.Brand = product.Brand;
                existingProduct.Price = product.Price;
                existingProduct.Discount = product.Discount;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.Image = product.Image;
                existingProduct.Weight = product.Weight;
                existingProduct.Length = product.Length;
                existingProduct.Breadth = product.Breadth;
                existingProduct.Height = product.Height;
                existingProduct.IsFeatured = product.IsFeatured;
                existingProduct.IsPopular = product.IsPopular;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating the product: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel> DeleteProduct(int id)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    return new ResponseModel
                    {
                        StatusCode = 404,
                        Message = "Product not found."
                    };
                }

                product.IsDeleted = true;
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    StatusCode = 200,
                    Message = "Product deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    StatusCode = 500,
                    Message = $"An error occurred while deleting the product: {ex.Message}"
                };
            }
        }
    }
}
