using Events.Inventory;
using Infrastructure.MessageBrokers;

namespace Inventory.Consumers
{
    public class InventoryTopologyConfigurator : TopologyConfigurator
    {
        public InventoryTopologyConfigurator()
        {
            AddProducer<InventorySpendEvent>();
        }
    }
}
