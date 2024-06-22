﻿using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContextDesignFactory()
        {

        }
        public OrderDbContext CreateDbContext(string[] args)
        {
            var connStr = "Server=DESKTOP-7UT1GE2\\SQLEXPRESS;Database=order;TrustServerCertificate=true;Encrypt=false;Trusted_Connection=True;";
            var optionsBuilder=new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlServer(connStr);

            return new OrderDbContext(optionsBuilder.Options,new NoMediator());
        }


    }
    class NoMediator : IMediator
    {

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            
            return AsyncEnumerable.Empty<TResponse>();

            // Implement the actual logic for creating the stream here
            // For example:
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            return AsyncEnumerable.Empty<object>();

        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<TResponse>(default);
        }


        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(default);

        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }
    }
}
