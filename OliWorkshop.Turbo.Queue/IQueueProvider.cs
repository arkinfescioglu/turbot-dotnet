using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OliWorkshop.Turbo.Queue
{
    public interface IQueueProvider
    {
        public Task Store(string queue, QueuePriority priority, object item);

        public Task<Tuple<string, QueuePriority>> Fetch(string queue, int batch);
        
        public Task<Tuple<string, QueuePriority>> Fetch(string queue, int batch, QueueItemState state);
        
        public Task<string> Fetch(string queue, QueuePriority priority, int batch, QueueItemState state);
    }
}
