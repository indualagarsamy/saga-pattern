using System;
using NServiceBus;

namespace Events
{
    public class CustomerWasBilled :
        IEvent
    {
        public Guid CustomerId { get; set; }
        public double DollarsPaid { get; set; }
    }
}
