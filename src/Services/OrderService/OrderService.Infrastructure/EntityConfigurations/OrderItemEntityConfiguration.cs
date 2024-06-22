using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems",OrderDbContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.Id);
            builder.Ignore(oi => oi.DomainEvents);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property<int>("OrderId").IsRequired();
        }
    }
}
