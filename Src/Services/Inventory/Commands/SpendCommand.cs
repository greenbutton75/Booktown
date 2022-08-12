using Events.Inventory;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using Infrastructure.Core.Exceptions;
using Inventory.Models;
using Inventory.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Commands
{
    public class SpendCommand
    {
        public class Command : ICommand
        {
            public InventoryItem item   { get; set; }
            public Command(InventoryItem item)
            {
                this.item = item;
            }
        }



        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
              RuleFor(x => x.item.ProductId).NotEmpty();
		      RuleFor(x => x.item.Quantity).GreaterThan(0);
            }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly IInventoryRepository _repository;
            private readonly IEventBus _eventBus;


            public Handler(IInventoryRepository repository, IEventBus eventBus)
            {
                _repository = repository;
                _eventBus = eventBus;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var dbitem = await _repository.GetItem(command.item);

                if (dbitem != null)
                {
                    if (dbitem.Quantity < command.item.Quantity)
                    {
                        throw new ArgumentException($"Try to spend {command.item.Quantity} of {command.item.ProductId} but have only {dbitem.Quantity}");
                    }

                    dbitem.Quantity = dbitem.Quantity - command.item.Quantity;

                    // If product now is out of stock - send event to Catalog
                    if (dbitem.Quantity == 0)
                    {
                            var @event = new InventoryOutOfStockEvent
                            {
                                ProductId = command.item.ProductId
                            };
                            await _eventBus.Commit(@event);
                    }

                    await _repository.UpdateItem(dbitem);

                }
                else
                {
                    throw new NotFoundException(typeof(InventoryItem).Name, command.item.ProductId);
                }

                return Unit.Value;
            }
        }
    }
}
