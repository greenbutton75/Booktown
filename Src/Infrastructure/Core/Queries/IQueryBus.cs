using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Core.Queries
{
    public interface IQueryBus
    {
        Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
