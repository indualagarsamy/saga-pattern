using System;
using NServiceBus;

namespace Messages
{
    public class CustomerWasPromotedToGold : IEvent
    {
        public Guid CustomerId { get; set; }
    }
}
