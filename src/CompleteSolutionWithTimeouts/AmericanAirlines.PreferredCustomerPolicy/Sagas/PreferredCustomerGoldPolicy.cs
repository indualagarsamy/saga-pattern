using System;
using System.Threading.Tasks;
using NServiceBus;
using Events;

public class PreferredCustomerGoldPolicy :
    Saga<PreferredCustomerGoldPolicyData>,
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
        Console.WriteLine($"New flight plan was received for {message.CustomerId}, miles = {message.MilesFlown}");
        Data.CustomerId = message.CustomerId;
        Data.TotalMilesFlown += message.MilesFlown;
        if (CanCustomerBePromotedToGold())
        {
            Console.WriteLine($"Customer {message.CustomerId} can be promoted in the next calendar year...Waiting...");
            await RequestTimeout(context, TimeSpan.FromSeconds(15), new CalendarYearHasStarted()).ConfigureAwait(false);
        }
    }

    public async Task Handle(CustomerWasBilled message, IMessageHandlerContext context)
    {
        Console.WriteLine($"New billing info was received for {message.CustomerId}, dollars = {message.DollarsPaid}");
        Data.CustomerId = message.CustomerId;
        Data.TotalDollarsPaid += message.DollarsPaid;
        if (CanCustomerBePromotedToGold())
        {
            Console.WriteLine($"Customer {message.CustomerId} can be promoted in the next calendar year...Waiting...");
            await RequestTimeout(context, TimeSpan.FromSeconds(15), new CalendarYearHasStarted()).ConfigureAwait(false);
        }
    }

    bool CanCustomerBePromotedToGold()
    {
        return Data.TotalMilesFlown >= 25000 &&
               Data.TotalDollarsPaid >= 3000;
    }

    public async Task Timeout(CalendarYearHasStarted state, IMessageHandlerContext context)
    {
        await context.Publish(new GoldStatusActivated
            {
                CustomerId = Data.CustomerId
            })
            .ConfigureAwait(false);
        Console.WriteLine("Publishing GoldStatusActivated event");
        MarkAsComplete();
    }
}