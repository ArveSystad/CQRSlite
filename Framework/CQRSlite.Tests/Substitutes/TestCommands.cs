using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain.Exception;
using CQRSlite.Messages;

namespace CQRSlite.Tests.Substitutes
{
    public class TestAggregateDoSomething : ICommand
    {
        public Guid Id { get; set; }
        public int ExpectedVersion { get; set; }
        public bool LongRunning { get; set; }
    }

    public class TestAggregateDoSomethingElse : ICommand
    {
        public Guid Id { get; set; }
        public int ExpectedVersion { get; set; }
        public bool LongRunning { get; set; }
    }

    public class TestAggregateDoSomethingHandler : ICancellableCommandHandler<TestAggregateDoSomething>
    {
        public async Task Handle(TestAggregateDoSomething command, CancellationToken token)
        {
            if (command.LongRunning)
                await Task.Delay(50, token);
            if(command.ExpectedVersion != TimesRun)
                throw new ConcurrencyException(command.Id);
            TimesRun++;
            Token = token;
        }

        public int TimesRun { get; set; }
        public CancellationToken Token { get; set; }

    }
	public class TestAggregateDoSomethingElseHandler : AbstractTestAggregateDoSomethingElseHandler
    {
        public override Task Handle(TestAggregateDoSomethingElse command)
        {
            TimesRun++;
            return Task.CompletedTask;
        }

        public int TimesRun { get; set; }
    }

    public abstract class AbstractTestAggregateDoSomethingElseHandler : ICommandHandler<TestAggregateDoSomethingElse>
    {
        public abstract Task Handle(TestAggregateDoSomethingElse command);
    }

    public class TestAggregateDoSomethingHandlerExplicit : ICommandHandler<TestAggregateDoSomething>
    {
        public int TimesRun { get; set; }
        Task IHandler<TestAggregateDoSomething>.Handle(TestAggregateDoSomething message)
        {
            TimesRun++;
            return Task.CompletedTask;
        }
    }

    public class TestAggregateDoSomethingHandlerExplicit2 : ICommandHandler<TestAggregateDoSomething>
    {
        public int TimesRun { get; set; }
        Task IHandler<TestAggregateDoSomething>.Handle(TestAggregateDoSomething command)
        {
            TimesRun++;
            return Task.CompletedTask;
        }

        public Task Handle(TestAggregateDoSomething command)
        {
            throw new NotImplementedException();
        }
    }
}