using Infrastructure.Core.Events;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public interface IMyConsumer<in TMessage> : IConsumer<TMessage> where TMessage : class
    {
    }
}
