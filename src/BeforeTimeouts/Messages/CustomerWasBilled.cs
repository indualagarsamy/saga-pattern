using System;
using NServiceBus;

namespace Messages
{
    public class CustomerWasBilled : IEvent
    {
        public Guid CustomerId { get; set; }
        public double DollarsPaid { get; set; }
    }
}
