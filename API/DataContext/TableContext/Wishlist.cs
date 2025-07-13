namespace OnlineStoreWebAPI.DataContext;
public partial class Wishlist
{
    public int WishlistId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
