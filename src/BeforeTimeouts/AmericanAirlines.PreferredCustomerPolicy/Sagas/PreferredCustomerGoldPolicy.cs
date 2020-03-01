using System;
using System.Threading.Tasks;
using NServiceBus;
using Events;

namespace PreferredCustomerPolicy.Sagas
{

    public class PreferredCustomerGoldPolicy : Saga<PreferredCustomerGoldPolicyData>,
        IAmStartedByMessages<FlightPlanWasAdded>,
        IAmStartedByMessages<CustomerWasBilled>
    {

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PreferredCustomerGoldPolicyData> mapper)
        {
            mapper.ConfigureMapping<FlightPlanWasAdded>(message => message.CustomerId)
                .ToSaga(data => data.CustomerId);

            mapper.ConfigureMapping<CustomerWasBilled>(message => message.CustomerId)
                .ToSaga(data => data.CustomerId);
        }
        public async Task Handle(FlightPlanWasAdded message, IMessageHandlerContext context)
        {
            Console.WriteLine("New flight plan was received for {0}, miles = {1}", message.CustomerId, message.MilesFlown);
            Data.CustomerId = message.CustomerId;
            Data.TotalMilesFlown += message.MilesFlown;
            if (CanCustomerBePromotedToGold(message.CustomerId))
            {
                Console.WriteLine("Customer {0} is now promoted to Gold", message.CustomerId, message.MilesFlown);

                await context.Publish(new CustomerWasPromotedToGold(){CustomerId = message.CustomerId}).ConfigureAwait(false);
                MarkAsComplete();
            }
        }

        public async Task Handle(CustomerWasBilled message, IMessageHandlerContext context)
        {
            Console.WriteLine("New billing info was received for {0}, dollars = {1}", message.CustomerId, message.DollarsPaid);
            Data.CustomerId = message.CustomerId;
            Data.TotalDollarsPaid += message.DollarsPaid;
            if (CanCustomerBePromotedToGold(message.CustomerId))
            {
                await context.Publish(new CustomerWasPromotedToGold(){ CustomerId = message.CustomerId }).ConfigureAwait(false);
                MarkAsComplete();
            }
        }

        private bool CanCustomerBePromotedToGold(Guid messageCustomerId)
        {
            if (Data.TotalMilesFlown >= 25000 && Data.TotalDollarsPaid >= 3000)
            {
                return true;
            }
            return false;
        }
    }
}
