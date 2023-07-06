using System;
using CQRSlite.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSlite;

public static class ServiceCollectionExtensions
{
    public static void AddHandlersFromAssemblyOf<T>(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblyOf<T>()
            .AddClasses(c => c.AssignableTo(typeof(IHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        
        services.Scan(scan => scan.FromAssemblyOf<T>()
            .AddClasses(c => c.AssignableTo(typeof(ICancellableHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }

    public static void ValidateHandlers(this IServiceProvider serviceProvider)
    {
        // if (handlers.Count != 1)
        //     throw new InvalidOperationException($"Cannot send to more than one handler of {type.FullName}");
        
        // If there are more than one command handler for a command, an exception should be thrown
        
        
    }
}