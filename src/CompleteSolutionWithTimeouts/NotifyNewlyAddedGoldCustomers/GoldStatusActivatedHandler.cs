using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace NewlyAddedGoldCustomers
{
    class GoldStatusActivatedHandler : IHandleMessages<GoldStatusActivated>
    {
        public Task Handle(GoldStatusActivated message, IMessageHandlerContext context)
        {
            Console.WriteLine("Customer Gold Status is now active.");
            return Task.CompletedTask;
        }
    }
}
