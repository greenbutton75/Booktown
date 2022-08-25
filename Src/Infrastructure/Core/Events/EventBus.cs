using Infrastructure.EventStores;
using Infrastructure.MessageBrokers;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Core.Events
{
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly IEventListener _eventListener;

        public EventBus(IMediator mediator, IEventListener eventListener)
        {
            _mediator = mediator ?? throw new Exception($"Missing dependency '{nameof(IMediator)}'");
            _eventListener = eventListener;
        }

        public virtual async Task PublishLocal(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await _mediator.Publish(@event);
            }
        }

        public virtual async Task CommitAsync(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await SendToMessageBroker(@event);
            }
        }

        private async Task SendToMessageBroker(IEvent @event)
        {
            if (_eventListener != null)
            {
                await _eventListener.Publish(@event);
            }
            else
            {
                throw new ArgumentNullException("No event listener found");
            }
        }
    }
}
