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

            public Handler(IInventoryRepository repository)
            {
                _repository = repository;
            }
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {

                /*
                var movie = await _repository.Find(command.Id);

                if (movie is null)
                {
                    throw new ArgumentNullException($"Could not find {nameof(movie)} with id '{command.Id}'");
                }

                movie.DeleteMovie(command.UserId);

                await _repository.Delete(movie);
                */

                return Unit.Value;
            }
        }
    }
}
