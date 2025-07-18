﻿namespace OnlineStoreWebAPI.DataContext;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
