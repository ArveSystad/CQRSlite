using System.Threading;
using System.Threading.Tasks;

namespace CQRSlite.Commands
{
    /// <summary>
    /// Defines a handler for a command with a cancellation token.
    /// </summary>
    /// <typeparam name="T">Command type being handled</typeparam>
    public interface ICancellableCommandHandler<in T> where T : ICommand
    {
        /// <summary>
        ///  Handles a message
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="token">Cancellation token from sender</param>
        Task Handle(T command, CancellationToken token = default);
    }
}