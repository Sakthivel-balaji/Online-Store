namespace OnlineStoreWebAPI.DataContext;
public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Brand { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public int StockQuantity { get; set; }

    public string? Image { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Length { get; set; }

    public decimal? Breadth { get; set; }

    public decimal? Height { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsPopular { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
