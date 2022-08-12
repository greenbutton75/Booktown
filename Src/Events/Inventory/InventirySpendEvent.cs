using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Inventory
{
    public class InventorySpendEvent : Event
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        public class Validator : AbstractValidator<InventorySpendEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ProductId).NotEmpty();
                RuleFor(e => e.Quantity).GreaterThanOrEqualTo(1);
            }
        }
    }
}
