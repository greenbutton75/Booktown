using Events.Inventory;
using Infrastructure.MessageBrokers;
using MassTransit;

namespace Inventory.Consumers
{
    public class InventoryLoadConsumer : IConsumer<InventoryLoadEvent>
    {
        public InventoryLoadConsumer()
        {
        }
        public async Task Consume(ConsumeContext<InventoryLoadEvent> context)
        {
            var msg=context.Message;
            //return context.RespondAsync<IAddItemsResponse>(new { OrderId = context.Message.OrderId });

            // TODO REMOVE IT - for test ony
            await context.Publish<InventoryOutOfStockEvent>(new
            {
                ProductId = "2"
            }) ;

            /*
                        this.logger.Info($"Reserve stock to {context.Message.CorrelationId} was received");
                        await Task.Delay(2000);
                        this.UpdateOrderState(context.Message.Order);
                        await context.Publish<IStockReserved>(new
                        {
                            CorrelationId = context.Message.CorrelationId,
                            Order = context.Message.Order
                        });
            */
        }

    }
}
