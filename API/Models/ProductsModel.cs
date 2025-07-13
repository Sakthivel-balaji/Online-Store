namespace ModelService
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int StockQuantity { get; set; }
        public double? AverageRating { get; set; }
        public string? Image { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Breadth { get; set; }
        public decimal? Height { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsPopular { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class ProductsModel
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public double? AverageRating { get; set; }
        public string? Image { get; set; }
        public int StockQuantity { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsPopular { get; set; }
    }
}