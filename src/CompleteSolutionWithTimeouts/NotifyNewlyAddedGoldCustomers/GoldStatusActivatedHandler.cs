using System;
using System.Threading.Tasks;
using NServiceBus;
using Events;

class GoldStatusActivatedHandler :
    IHandleMessages<GoldStatusActivated>
{
    public Task Handle(GoldStatusActivated message, IMessageHandlerContext context)
    {
        Console.WriteLine("Customer Gold Status is now active.");
        return Task.CompletedTask;
    }
}