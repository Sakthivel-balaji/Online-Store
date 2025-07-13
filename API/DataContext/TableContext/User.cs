namespace OnlineStoreWebAPI.DataContext;

public partial class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public bool? IsDeleted { get; set; }
    public string? UserName { get; set; }
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
