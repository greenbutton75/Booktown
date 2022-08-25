using Events.Inventory;
using FluentValidation;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
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
    public class LoadCommand
    {
        public class Command : ICommand
        {
            public IEnumerable<InventoryItem> items   { get; set; }

            public Command(IEnumerable<InventoryItem> items)
            {
                this.items = items;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
		    RuleForEach(x => x.items).ChildRules(item => 
		    {
                      item.RuleFor(x => x.ProductId).NotEmpty();
		      item.RuleFor(x => x.Quantity).GreaterThan(0);
		    });
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
                foreach (var item in command.items)
                {
                    var dbitem = await _repository.GetItemAsync(item);

                    // If it is a new product or now we have it in stock - send event to Catalog
                    if (dbitem == null || dbitem.Quantity == 0)
                    {
                        var @event = new InventoryInStockEvent
                        {
                            ProductId = item.ProductId
                        };
                        await _eventBus.CommitAsync(@event);
                    }

                    if (dbitem != null)
                    {
                        dbitem.Quantity = dbitem.Quantity + item.Quantity;
                    }
                    else
                    {
                        dbitem = new InventoryItem { ProductId=item.ProductId, Quantity= item.Quantity };
                    }

                    await _repository.UpdateItemAsync(dbitem);
                }







                return Unit.Value;
            }
        }
    }
}
