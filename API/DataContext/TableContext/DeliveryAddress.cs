namespace OnlineStoreWebAPI.DataContext;

public partial class DeliveryAddress
{
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public bool IsPrimary { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
