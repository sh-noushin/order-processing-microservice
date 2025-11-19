using OrderProcessing.Core.Domain;

namespace OrderProcessing.Core.Abstractions
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(
            string customerId,
            IReadOnlyCollection<OrderItem> items,
            CancellationToken cancellationToken = default);

        Task<Order?> GetOrderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<int> CancelStalePendingOrdersAsync(
            TimeSpan maxPendingAge,
            CancellationToken cancellationToken = default);
    }
}
