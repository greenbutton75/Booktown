using Events.Inventory;
using Infrastructure.Core;
using Infrastructure.Core.Commands;
using Infrastructure.MessageBrokers;
using Inventory.Commands;
using Inventory.Models;
using MassTransit;

namespace Inventory.Consumers
{
    public class InventorySpendConsumer : IMyConsumer<InventorySpendEvent>
    {
        private readonly ICommandBus _commandBus;
        public InventorySpendConsumer(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }
        public async Task Consume(ConsumeContext<InventorySpendEvent> context)
        {
            var item = context.Message;

            var command = new SpendCommand.Command(Mapping.Map<InventorySpendEvent, InventoryItem>(item));

            await _commandBus.SendAsync(command);
        }

    }
}
