namespace ModelService
{
    public class AddressModel
    {
        public int? AddressId { get; set; }
        public int CustomerId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsPrimary { get; set; }
    }
}