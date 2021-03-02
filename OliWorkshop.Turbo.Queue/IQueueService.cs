using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Queue
{
    public interface IQueueService<TQueueContext, TQueueItem> 
        where TQueueContext : QueueContext
        where TQueueItem: ItemPrototype
    {
        public void Push(TQueueItem queue);

        public void Push(TQueueItem queue, QueuePriority priority);

        public Task<TQueueItem> Next();

        public IAsyncEnumerable<TQueueItem> ListenByCollection();
        
        public Channel<TQueueItem> ListenByChannel();

        public Task UpdateState(TQueueItem queue, QueueItemState state);


        public Task UpdateState(TQueueItem queue, QueuePriority priority);

        public Task Forget(string itemId);
    }
}
