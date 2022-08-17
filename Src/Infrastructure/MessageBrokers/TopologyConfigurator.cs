using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MessageBrokers
{
    public class TopologyConfigurator
    {
        private List<Action<IRiderRegistrationConfigurator>> ProducerRegistrations { get; set; } = new List<Action<IRiderRegistrationConfigurator>>();

        public void ConfigureProducers(IRiderRegistrationConfigurator rider)
        {
            foreach (var reg in ProducerRegistrations)
            {
                reg.Invoke(rider);
            }
        }
        protected void AddProducer<T>() where T : class
        {
            ProducerRegistrations.Add(rider => rider.AddProducer<T>(nameof(T)));
        }
    }
}
