using System;
using System.Collections.Generic;

namespace OrderIngest.Api.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public CustomerDto Customer { get; set; } = new();
        public string Currency { get; set; } = "USD";
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CustomerDto 
    { 
        public string Email { get; set; } = string.Empty; 
        public string? Name { get; set; } 
    }

    public class OrderItemDto 
    { 
        public string SKU { get; set; } = string.Empty; 
        public string? ProductName { get; set; } 
        public int Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
    }
}
