using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContextSeed
    {
        public async Task SeedAsync(OrderDbContext context, ILogger<OrderDbContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = false;
                var contentRootPath = "Seeding/Setup";

                using (context)
                {
                    context.Database.Migrate();
                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData
                            ? GetCardTypesFromFile(contentRootPath, logger)
                            : GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }
                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData
                          ? GetOrderStatusFromFile(contentRootPath, logger)
                          : GetPredefinedOrderStatus());

                        await context.SaveChangesAsync();
                    }
                }
            });
        }
        private IEnumerable<CardType>GetCardTypesFromFile(string contentRootPath, ILogger<OrderDbContext> logger)
        {
            string filename = "CardTypes.txt";
            if (!File.Exists(filename))
            {
                return GetPredefinedCardTypes();
            }
            var filecontent = File.ReadAllLines(filename);
            int id = 1;
            var list = filecontent.Select(i => new CardType(id++, i)).Where(i => i != null);
            return list;
        }
        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }
        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentPath, ILogger<OrderDbContext> logger)
        {
            string filename = "OrderStatus.txt";
            if (!File.Exists(filename))
            {
                return GetPredefinedOrderStatus();
            }
            var fileContent = File.ReadAllLines(filename);
            int id= 1;
            var list = fileContent.Select(i => new OrderStatus(id++, i)).Where(i => i != null);
            return list;
        }
        private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>()
            {
                OrderStatus.Submitted,
                OrderStatus.AwaitingValidation,
                OrderStatus.StockConfirmed,
                OrderStatus.Paid,
                OrderStatus.Shipped,
                OrderStatus.Canceled
            };

        }
        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContext> logger,string prefix,int retries = 3)
        {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount:retries,
                    sleepDurationProvider:retry=>TimeSpan.FromSeconds(5),
                    onRetry: (exception, timespan, retry, ctx) =>
                    {
                        logger.LogWarning(exception,"[{prefix}] Exception {ExceptionType} with message {Message}");
                    }
                );
        }

    }
}
