using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Core.Commands
{
    public interface ICommandBus
    {
        Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default(CancellationToken));

        Task SendAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
