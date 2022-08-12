using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Inventory
{
    public class InventoryInStockEvent : Event
    {
        public string ProductId { get; set; }

        public class Validator : AbstractValidator<InventoryInStockEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ProductId).NotEmpty();
            }
        }
    }
}

