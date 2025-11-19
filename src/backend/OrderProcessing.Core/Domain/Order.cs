namespace OrderProcessing.Core.Domain
{
    public class Order
    {
        public Guid Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalAmount => Items.Sum(i => i.LineTotal);
    }
}
