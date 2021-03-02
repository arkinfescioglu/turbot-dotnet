using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.Queue
{
    public enum QueueItemState
    {
        Pedding,
        Success,
        Processing,
        Failed,
        Skipped
    }
}
