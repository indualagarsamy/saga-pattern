using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NServiceBus.Testing;
using Events;

[TestFixture]
public class PreferredStatusGoldTests
{
    PreferredCustomerGoldPolicy saga;
    TestableMessageHandlerContext context;

    [SetUp]
    public void Setup()
    {
        saga = new PreferredCustomerGoldPolicy
        {
            Data = new PreferredCustomerGoldPolicyData()
        };
        context = new TestableMessageHandlerContext();
    }

    [Test]
    public async Task WaitForCalendarYearBeforeUpgradingPreferredStatusToGold()
    {
        var customerId = Guid.NewGuid();

        var flightPlanWasAdded = new FlightPlanWasAdded
        {
            CustomerId = customerId,
            MilesFlown = 25000
        };
        var customerWasBilled = new CustomerWasBilled
        {
            CustomerId = customerId,
            DollarsPaid = 3000
        };
        await saga.Handle(flightPlanWasAdded, context);
        await saga.Handle(customerWasBilled, context);

        // also test how long the timeout was supposed to be.
        Assert.AreEqual(TimeSpan.FromSeconds(15), context.TimeoutMessages[0].Within);
        Assert.IsFalse(saga.Completed);
        Assert.AreEqual(0, context.PublishedMessages.Length);
    }

    [Test]
    public async Task ShouldMakeCustomersPreferredStatusToGold()
    {
        var customerId = Guid.NewGuid();
        var flightPlanWasAdded = new FlightPlanWasAdded
        {
            CustomerId = customerId,
            MilesFlown = 25000
        };
        var customerWasBilled = new CustomerWasBilled
        {
            CustomerId = customerId,
            DollarsPaid = 3000
        };
        await saga.Handle(flightPlanWasAdded, context);
        await saga.Handle(customerWasBilled, context);

        await saga.Timeout(new CalendarYearHasStarted(), context);

        Assert.IsTrue(saga.Completed);
        Assert.AreEqual(1, context.PublishedMessages.Length);
    }

    [Test]
    public async Task ShouldNotMakeCustomersPreferredStatusToGoldWhenNotEnoughMilesHasBeenFlown()
    {
        var customerId = Guid.NewGuid();
        var flightPlanWasAdded = new FlightPlanWasAdded
        {
            CustomerId = customerId,
            MilesFlown = 2500
        };
        var customerWasBilled = new CustomerWasBilled
        {
            CustomerId = customerId,
            DollarsPaid = 3500
        };
        await saga.Handle(flightPlanWasAdded, context);
        await saga.Handle(customerWasBilled, context);

        Assert.IsFalse(saga.Completed);
        Assert.AreEqual(0, context.PublishedMessages.Length);
    }

    [Test]
    public async Task ShouldNotMakeCustomersPreferredStatusToGoldWhenNotEnoughDollarsHasBeenSpent()
    {
        var customerId = Guid.NewGuid();
        var flightPlanWasAdded = new FlightPlanWasAdded
        {
            CustomerId = customerId,
            MilesFlown = 35000
        };
        var customerWasBilled = new CustomerWasBilled
        {
            CustomerId = customerId,
            DollarsPaid = 1000
        };
        await saga.Handle(flightPlanWasAdded, context);
        await saga.Handle(customerWasBilled, context);

        Assert.IsFalse(saga.Completed);
        Assert.AreEqual(0, context.PublishedMessages.Length);
    }
}