using System;
using System.Threading.Tasks;
using NServiceBus;
using Events;

public class PreferredCustomerGoldPolicy :
    Saga<PreferredCustomerGoldPolicyData>,
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
        Console.WriteLine($"New flight plan was received for {message.CustomerId}, miles = {message.MilesFlown}");
        Data.CustomerId = message.CustomerId;
        Data.TotalMilesFlown += message.MilesFlown;
        if (CanCustomerBePromotedToGold())
        {
            Console.WriteLine($"Customer {message.CustomerId} is now promoted to Gold");

            await context.Publish(
                new CustomerWasPromotedToGold
                {
                    CustomerId = message.CustomerId
                }).ConfigureAwait(false);
            MarkAsComplete();
        }
    }

    public async Task Handle(CustomerWasBilled message, IMessageHandlerContext context)
    {
        Console.WriteLine($"New billing info was received for {message.CustomerId}, dollars = {message.DollarsPaid}");
        Data.CustomerId = message.CustomerId;
        Data.TotalDollarsPaid += message.DollarsPaid;
        if (CanCustomerBePromotedToGold())
        {
            await context.Publish(
                    new CustomerWasPromotedToGold
                    {
                        CustomerId = message.CustomerId
                    })
                .ConfigureAwait(false);
            MarkAsComplete();
        }
    }

    bool CanCustomerBePromotedToGold()
    {
        return Data.TotalMilesFlown >= 25000 &&
               Data.TotalDollarsPaid >= 3000;
    }
}