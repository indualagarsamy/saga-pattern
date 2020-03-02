using System;
using System.Threading.Tasks;
using NServiceBus;
using Events;

namespace PreferredCustomerPolicy.Sagas
{

    public class PreferredCustomerGoldPolicy : Saga<PreferredCustomerGoldPolicyData>,
        IAmStartedByMessages<FlightPlanWasAdded>,
        IAmStartedByMessages<CustomerWasBilled>,
        IHandleTimeouts<CalendarYearHasStarted>
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
                Console.WriteLine("Customer {0} can be promoted in the next calendar year...Waiting...", message.CustomerId);
                await RequestTimeout(context, TimeSpan.FromSeconds(15), new CalendarYearHasStarted()).ConfigureAwait(false);
            }
        }

        public async Task Handle(CustomerWasBilled message, IMessageHandlerContext context)
        {
            Console.WriteLine("New billing info was received for {0}, dollars = {1}", message.CustomerId, message.DollarsPaid);
            Data.CustomerId = message.CustomerId;
            Data.TotalDollarsPaid += message.DollarsPaid;
            if (CanCustomerBePromotedToGold(message.CustomerId))
            {
                Console.WriteLine("Customer {0} can be promoted in the next calendar year...Waiting...", message.CustomerId);
                await RequestTimeout(context, TimeSpan.FromSeconds(15), new CalendarYearHasStarted()).ConfigureAwait(false);
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

        public async Task Timeout(CalendarYearHasStarted state, IMessageHandlerContext context)
        {
            await context.Publish(new GoldStatusActivated() { CustomerId = Data.CustomerId }).ConfigureAwait(false);
            Console.WriteLine("Publishing GoldStatusactivated event");
            MarkAsComplete();
        }
    }

    public class CalendarYearHasStarted
    {
    }
}
