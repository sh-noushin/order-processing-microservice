using OrderProcessing.Core.Abstractions;
using OrderProcessing.Core.Domain;

namespace OrderProcessing.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository
                ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Order> CreateOrderAsync(
            string customerId,
            IReadOnlyCollection<OrderItem> items,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("CustomerId is required.", nameof(customerId));

            if (items == null || items.Count == 0)
                throw new ArgumentException("At least one order item is required.", nameof(items));

            if (items.Any(i => i.Quantity <= 0))
                throw new ArgumentException("All items must have Quantity > 0.", nameof(items));

            if (items.Any(i => i.UnitPrice < 0))
                throw new ArgumentException("All items must have UnitPrice >= 0.", nameof(items));

            var orderId = Guid.NewGuid();

            var orderItems = items
                .Select(input => new OrderItem
                {
                    Id = input.Id == Guid.Empty ? Guid.NewGuid() : input.Id,
                    OrderId = orderId,
                    ProductId = input.ProductId,
                    Quantity = input.Quantity,
                    UnitPrice = input.UnitPrice
                })
                .ToList();

            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                CreatedAtUtc = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = orderItems
            };

            await _orderRepository.AddAsync(order, cancellationToken);

            return order;
        }

        public Task<Order?> GetOrderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return _orderRepository.GetByIdAsync(orderId, cancellationToken);
        }

        public async Task<int> CancelStalePendingOrdersAsync(
            TimeSpan maxPendingAge,
            CancellationToken cancellationToken = default)
        {
            if (maxPendingAge <= TimeSpan.Zero)
                throw new ArgumentException("Max pending age must be positive.", nameof(maxPendingAge));

            var threshold = DateTime.UtcNow - maxPendingAge;

            var staleOrders = await _orderRepository.GetPendingOrdersOlderThanAsync(threshold, cancellationToken);

            foreach (var order in staleOrders)
            {
                if (order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Canceled;
                    await _orderRepository.UpdateAsync(order, cancellationToken);
                }
            }

            return staleOrders.Count;
        }
    }
}
