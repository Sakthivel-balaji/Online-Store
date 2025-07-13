namespace ModelService
{
    public class CustomersModel
    {
        public int? CustomerId { get; set; }
        public string? Email { get ; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
    
    public class CustomerModel
    {
        public int? CustomerId { get; set; }
        public string? Email { get ; set; }
        public string? ProfilePicture { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CustomerUpdateModel
    {
        public int? CustomerId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}