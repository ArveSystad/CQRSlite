using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain.Exception;
using CQRSlite.Routing;
using CQRSlite.Tests.Substitutes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CQRSlite.Tests.Routing
{
    public class When_publishing_events
    {
        [Fact]
        public async Task Should_publish_to_all_handlers()
        {
            var handler = new TestAggregateDidSomethingHandler();
            var handler2 = new TestAggregateDidSomethingHandler();
            
            var services = new ServiceCollection();
            services.AddSingleton(handler);
            services.AddSingleton(handler2);
            var serviceProvider = services.BuildServiceProvider();

            var router = new Router(serviceProvider);

            await router.Publish(new TestAggregateDidSomethingElse());
            
            handler.TimesRun.Should().Be(1);
            handler2.TimesRun.Should().Be(1);
        }

        [Fact]
        public async Task Should_publish_to_all_cancellation_handlers()
        {
            var handler = new TestAggregateDidSomethingCancellableHandler();
            var handler2 = new TestAggregateDidSomethingCancellableHandler();
            
            var services = new ServiceCollection();
            services.AddSingleton(handler);
            services.AddSingleton(handler2);
            var serviceProvider = services.BuildServiceProvider();

            var router = new Router(serviceProvider);

            await router.Publish(new TestAggregateDidSomethingElse());
            
            handler.TimesRun.Should().Be(1);
            handler2.TimesRun.Should().Be(1);
        }
        
        [Fact]
        public async Task Should_work_with_no_handlers()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();

            var router = new Router(serviceProvider);
            await router.Publish(new TestAggregateDidSomething());
        }
        
        [Fact]
        public async Task Should_throw_if_handler_throws()
        {
            var services = new ServiceCollection();
            services.AddTransient<TestAggregateDidSomethingHandler>();
            var serviceProvider = services.BuildServiceProvider();
            var router = new Router(serviceProvider);
            
            await Assert.ThrowsAsync<ConcurrencyException>(async () => await router.Publish(new TestAggregateDidSomething {Version = -10}));
        }
        
        [Fact]
        public async Task Should_wait_for_published_to_finish()
        {
            var handler = new TestAggregateDidSomethingHandler();
            _router.RegisterHandler<TestAggregateDidSomething>(handler.Handle);
            await _router.Publish(new TestAggregateDidSomething {LongRunning = true});
            Assert.Equal(1, handler.TimesRun);
        }
        
        [Fact]
        public async Task Should_forward_cancellation_token()
        {
            var token = new CancellationToken();
            var handler = new TestAggregateDidSomethingHandler();
            _router.RegisterHandler<TestAggregateDidSomething>(handler.Handle);
            await _router.Publish(new TestAggregateDidSomething {LongRunning = true}, token);
            Assert.Equal(token, handler.Token);
        }
    }
}