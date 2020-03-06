using System;
using NServiceBus;

namespace Events
{
    public class FlightPlanWasAdded :
        IEvent
    {
        public Guid CustomerId { get; set; }
        public int MilesFlown { get; set; }
    }
}