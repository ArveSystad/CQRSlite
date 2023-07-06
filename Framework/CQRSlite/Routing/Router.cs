using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSlite.Routing
{
    /// <summary>
    /// Default router implementation for sending commands and publishing events.
    /// </summary>
    public class Router : ICommandSender, IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public Router(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task Send<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            using (var scope = _serviceProvider.CreateScope())
            {
                var commandHandler = scope.ServiceProvider.GetService(handlerType);

                if (commandHandler == null)
                {
                    var cancellableHandlerType = typeof(ICancellableCommandHandler<>).MakeGenericType(command.GetType());
                    commandHandler = scope.ServiceProvider.GetService(cancellableHandlerType);
                }
                
                if(commandHandler == null)
                    throw new InvalidOperationException($"No handler registered for {handlerType.FullName}");
                
                var task = (Task)handlerType.InvokeMember(nameof(ICancellableCommandHandler<ICommand>.Handle), BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, commandHandler, new object[] { command, cancellationToken });
                await task;    
            }
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
        {
            var handlerType = typeof(IHandler<>).MakeGenericType(@event.GetType());
            var cancellablehandlerType = typeof(IHandler<>).MakeGenericType(@event.GetType());

            using (var scope = _serviceProvider.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();
                handlers.AddRange(scope.ServiceProvider.GetServices(cancellablehandlerType));
                
                if(handlers == null || !handlers.Any())
                    throw new InvalidOperationException($"No handler registered for {handlerType.FullName}");

                var tasks = handlers.Select(handler => 
                    (Task)handlerType.InvokeMember(nameof(IHandler<IMessage>.Handle), BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, handler, new object[] { @event, cancellationToken })
                );
                await Task.WhenAll(tasks);
            }
        }
    }
}
