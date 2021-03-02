using System;

namespace OliWorkshop.Turbo.Queue
{
    public abstract class ItemPrototype
    {
        public Guid Id { get; set; }

        public QueueItemState state { get; set; }

        public QueuePriority priority { get; set; }
    }
}