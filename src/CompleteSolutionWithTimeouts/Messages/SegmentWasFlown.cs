using System;
using NServiceBus;

namespace Messages
{
    public class SegmentWasFlown : IEvent
    {
        public Guid CustomerId { get; set; }
        public int MilesFlown { get; set; }
    }
}
