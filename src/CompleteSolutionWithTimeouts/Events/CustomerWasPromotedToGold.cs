using System;
using NServiceBus;

namespace Events
{
    public class CustomerWasPromotedToGold :
        IEvent
    {
        public Guid CustomerId { get; set; }
    }
}
