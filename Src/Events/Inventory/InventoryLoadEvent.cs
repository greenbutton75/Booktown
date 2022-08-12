using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Inventory
{
    public class InventoryLoadEvent : Event
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        public class Validator : AbstractValidator<InventoryLoadEvent>
        {
            public Validator()
            {
                RuleFor(e => e.ProductId).NotEmpty();
                RuleFor(e => e.Quantity).GreaterThanOrEqualTo(1);
            }
        }
    }
}
