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
                var @event = new InventoryLoadEvent { ProductId="1", Quantity=10};
                await _eventBus.Commit(@event);

                //command.items  TODO
                await _repository.DoNothing();


                return Unit.Value;
            }
        }
    }
}
