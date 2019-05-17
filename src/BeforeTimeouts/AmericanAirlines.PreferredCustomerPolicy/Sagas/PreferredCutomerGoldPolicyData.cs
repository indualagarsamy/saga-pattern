using System;
using NServiceBus;

namespace PreferredCustomerPolicy.Sagas
{
    public class PreferredCutomerGoldPolicyData : ContainSagaData
    {
        public Guid CustomerId { get; set; }
        public int TotalMilesFlown { get; set; }
        public double TotalDollarsPaid { get; set; }
    }
}
