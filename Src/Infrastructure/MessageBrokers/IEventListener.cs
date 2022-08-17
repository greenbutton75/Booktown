using Infrastructure.Core.Events;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public interface IEventListener
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}
