using Infrastructure.EventStores;
using System.Threading.Tasks;

namespace Infrastructure.Core.Events
{
    public interface IEventBus
    {
        Task PublishLocal(params IEvent[] events);
        Task CommitAsync(params IEvent[] events);
    }
}
