namespace OnlineStoreWebAPI.DataContext;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public int? AddressId { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual DeliveryAddress? Address { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
