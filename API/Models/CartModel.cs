namespace ModelService
{
    public class CartModel
    {
        public int CustomerId { get; set; }
        public decimal? ItemSubTotal { get; set; }
        public decimal? TotalDiscountPrice { get; set; }
        public decimal? SubTotal { get; set; }
        public List<CartProductsModel>? Products { get; set; }
    }

    public class CartProductsModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? Discount { get; set; }
        public int StockQuantity { get; set; }
        public int Quantity { get; set; }
        public decimal? PriceBeforeDiscount { get; set; }
        public decimal? PriceAfterDiscount { get; set; }
    }

    public class CartUpdateModel
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartInsertModel
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}