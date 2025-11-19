using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Abstractions;
using OrderProcessing.Core.Services;
using OrderProcessing.Infrastructure;
using OrderProcessing.Infrastructure.Repositories;

namespace OrderProcessing.Api.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("OrdersDb")
                ?? "Data Source=orders.db";

            services.AddDbContext<OrdersDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IOrderRepository, EfOrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
