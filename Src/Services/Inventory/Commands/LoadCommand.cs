using FluentValidation;
using Infrastructure.Core.Commands;
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

            public Handler(IInventoryRepository repository)
            {
                _repository = repository;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {

                //command.items  TODO
                await _repository.DoNothing();


                return Unit.Value;
            }
        }
    }
}
