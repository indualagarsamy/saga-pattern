using System;
using NServiceBus;

namespace Events
{
    public class GoldStatusActivated : IEvent
    {
        public Guid CustomerId { get; set; }
    }
}
