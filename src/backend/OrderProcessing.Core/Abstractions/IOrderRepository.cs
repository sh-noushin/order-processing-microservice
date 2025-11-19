using OrderProcessing.Core.Domain;

namespace OrderProcessing.Core.Abstractions
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Order order,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetPendingOrdersOlderThanAsync(
            DateTime utcThreshold,
            CancellationToken cancellationToken = default);
    }
}
