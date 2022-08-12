using System;

namespace Infrastructure.Core.Events
{
    public abstract class Event : IEvent
    {
        public virtual Guid Id { get; } = Guid.NewGuid();
        public virtual Guid CorrelationId { get; set; }
        public virtual DateTime CreatedUtc { get; } = DateTime.UtcNow;
    }
}
