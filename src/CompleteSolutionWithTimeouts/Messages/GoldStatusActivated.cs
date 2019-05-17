using System;
using NServiceBus;

namespace Messages
{
    public class GoldStatusActivated : IEvent
    {
        public Guid CustomerId { get; set; }
    }
}
