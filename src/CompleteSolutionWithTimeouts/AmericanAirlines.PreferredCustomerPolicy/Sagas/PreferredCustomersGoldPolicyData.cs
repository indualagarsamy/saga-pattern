using System;
using NServiceBus;

namespace PreferredCustomerPolicy.Sagas
{
    public class PreferredCustomersGoldPolicyData : ContainSagaData
    {
        public Guid CustomerId { get; set; }
        public int TotalMilesFlown { get; set; }
        public double TotalDollarsPaid { get; set; }
    }
}
