using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Abstractions;
using OrderProcessing.Core.Domain;

namespace OrderProcessing.Infrastructure.Repositories
{
    public class EfOrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _dbContext;

        public EfOrderRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Order?> GetByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetPendingOrdersOlderThanAsync(
            DateTime utcThreshold,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Pending &&
                            o.CreatedAtUtc < utcThreshold)
                .ToListAsync(cancellationToken);
        }
    }
}
