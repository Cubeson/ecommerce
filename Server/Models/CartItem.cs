﻿namespace Server.Models;

public class CartItem
{
    public int Id { get; set; }
    public required int Quantity { get; set; }
    public virtual Product Product { get; set; }
    public required int ProductId { get; set; }
}
