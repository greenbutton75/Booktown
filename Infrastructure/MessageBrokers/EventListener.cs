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

        public virtual void Subscribe<T>() where T : IEvent
        {
            Subscribe(typeof(T));
        }

        public virtual void Subscribe(Type type)
        {
/*
            using (var consumer = new ConsumerBuilder<string, string>(_kafkaOptions.Consumer).Build())
            {
                consumer.Subscribe(_topics);
                while (true)
                {
                    var message = consumer.Consume();

                    var @event = JsonConvert.DeserializeObject(message.Message.Value, type) as IEvent;

                    using (var scope = _serviceFactory.CreateScope())
                    {
                        var eventBus = scope.ServiceProvider.GetService<IEventBus>();
                        eventBus.PublishLocal(@event);
                    }
                }
            }
*/
        }

        public virtual async Task Publish<T>(T @event) where T : IEvent
        {
            await _publishEndpoint.Publish(@event, typeof(T));
        }
    }
}
