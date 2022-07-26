using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Inventory
{
    public class InventoryOutOfStockEvent : Event
    {
        public string ProductId { get; set; }

        public class Validator : AbstractValidator<InventoryOutOfStockEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ProductId).NotEmpty();
            }
        }
    }
}
