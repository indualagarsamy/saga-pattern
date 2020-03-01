using System;
using NServiceBus;

namespace PreferredCustomerPolicy.Sagas
{
    public class PreferredCustomerGoldPolicyData : ContainSagaData
    {
        public Guid CustomerId { get; set; }
        public int TotalMilesFlown { get; set; }
        public double TotalDollarsPaid { get; set; }
    }
}
