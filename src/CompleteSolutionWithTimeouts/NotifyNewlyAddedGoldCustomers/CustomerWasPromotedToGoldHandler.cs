using System;
using System.Threading.Tasks;
using Events;
using NServiceBus;

class CustomerWasPromotedToGoldHandler :
    IHandleMessages<CustomerWasPromotedToGold>
{
    public Task Handle(CustomerWasPromotedToGold message, IMessageHandlerContext context)
    {
        Console.WriteLine("Notify the customer about gold status");
        return Task.CompletedTask;
    }
}