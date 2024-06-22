using System;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Features.Commands.CreateOrder;
using OrderService.Application.Mapping.OrderMapping;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, Type startup)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(executingAssembly);

        services.AddMediatR(executingAssembly);

        return services;
    }
}
