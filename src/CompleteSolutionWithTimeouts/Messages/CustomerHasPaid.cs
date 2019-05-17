using System;
using NServiceBus;

namespace Messages
{
    public class CustomerHasPaid : IEvent
    {
        public Guid CustomerId { get; set; }
        public double DollarsPaid { get; set; }
    }
}
