using System;
using NServiceBus;

namespace Messages
{
    public class FlightPlanWasAdded : IEvent
    {
        public Guid CustomerId { get; set; }
        public int MilesFlown { get; set; }
    }
}
