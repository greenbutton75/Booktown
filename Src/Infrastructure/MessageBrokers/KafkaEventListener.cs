using Infrastructure.Core.Events;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public class KafkaEventListener : IEventListener
    {
        //private readonly IServiceScopeFactory _serviceFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public KafkaEventListener(IServiceProvider serviceProvider, IPublishEndpoint publishEndpoint)
        {
            _serviceProvider = serviceProvider;
            _publishEndpoint=publishEndpoint;
        }


        public virtual async Task Publish<T>(T @event) where T : class, IEvent
        {
            ///await _publishEndpoint.Publish(@event);

            var _topicProducer = _serviceProvider.GetRequiredService<ITopicProducer<T>>();
            await _topicProducer.Produce(@event);

        }
    }
}
