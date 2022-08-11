using Infrastructure.Core.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public class EventListener : IEventListener
    {
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly IPublishEndpoint _publishEndpoint;

        public EventListener(IServiceScopeFactory serviceFactory, IPublishEndpoint publishEndpoint)
        {
            _serviceFactory = serviceFactory;
            _publishEndpoint=publishEndpoint;
        }


        public virtual async Task Publish<T>(T @event) where T : IEvent
        {
            await _publishEndpoint.Publish(@event, @event.GetType());
        }
    }
}
