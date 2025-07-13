namespace ModelService
{
    public class OrderModel
    {
        public int? OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public DeliveryModel? DeliveryInfo { get; set; }
        public List<ProductInfoModel>? Products { get; set; }
    }

    public class OrdersModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class OrderInsertModel
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
    }

    public class OrderUpdateModel
    {
        public int OrderId { get ; set ; }
        public string? Status { get ; set ; }
        public DateTime? DeliveryDate { get ; set ; }
    }

    public class ProductInfoModel
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Image { get; set; }
    }

    public class DeliveryModel
    {
        public int AddressId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
    }
}