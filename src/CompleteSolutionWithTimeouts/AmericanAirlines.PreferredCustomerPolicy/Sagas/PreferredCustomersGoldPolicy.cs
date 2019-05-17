using System;
using System.Threading.Tasks;
using NServiceBus;
using Messages;

namespace PreferredCustomerPolicy.Sagas
{

    class PreferredCustomersGoldPolicy : Saga<PreferredCustomersGoldPolicyData>, 
        IAmStartedByMessages<SegmentWasFlown>,
        IAmStartedByMessages<CustomerHasPaid>,
        IHandleTimeouts<CalendarYearHasStarted>
    {

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PreferredCustomersGoldPolicyData> mapper)
        {
            mapper.ConfigureMapping<SegmentWasFlown>(message => message.CustomerId)
                .ToSaga(data => data.CustomerId);

            mapper.ConfigureMapping<CustomerHasPaid>(message => message.CustomerId)
                .ToSaga(data => data.CustomerId);
        }
        public async Task Handle(SegmentWasFlown message, IMessageHandlerContext context)
        {
            Console.WriteLine("New flight plan was received for {0}, miles = {1}", message.CustomerId, message.MilesFlown);
            Data.CustomerId = message.CustomerId;
            Data.TotalMilesFlown += message.MilesFlown;
           
            if (CanCustomerBePromotedToGold(message.CustomerId))
            {
                Console.WriteLine("Customer {0} is now promoted to Gold", message.CustomerId, message.MilesFlown);

                await context.Publish(new CustomerWasPromotedToGold(){CustomerId = message.CustomerId}).ConfigureAwait(false);
                await RequestTimeout(context, TimeSpan.FromSeconds(15), new CalendarYearHasStarted()).ConfigureAwait(false);
            }
        }
        

        public async Task Handle(CustomerHasPaid message, IMessageHandlerContext context)
        {
            Console.WriteLine("New payment info was received for {0}, dollars = {1}", message.CustomerId, message.DollarsPaid);
            Data.CustomerId = message.CustomerId;
            Data.TotalDollarsPaid += message.DollarsPaid;
            if (CanCustomerBePromotedToGold(message.CustomerId))
            {
                await context.Publish(new CustomerWasPromotedToGold()).ConfigureAwait(false);
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
            await context.Publish(new GoldStatusActivated() {CustomerId = Data.CustomerId}).ConfigureAwait(false);
            Console.WriteLine("Publishing GoldStatusactivated event");
        }
    }

    public class CalendarYearHasStarted
    {
    }
}
