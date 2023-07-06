using System.Threading.Tasks;

namespace CQRSlite.Commands
{
    /// <summary>
    /// Defines a handler for a command.
    /// </summary>
    /// <typeparam name="T">Command being handled</typeparam>
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command);
    }
}