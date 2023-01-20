using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public OrderItemDTO[]? Items { get; set; }
        public decimal Total { get; set; }

    }
}
