using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace NotifyNewlyAddedGoldCustomers
{
    class CustomerWasPromotedToGoldHandler : IHandleMessages<CustomerWasPromotedToGold>
    {
        public Task Handle(CustomerWasPromotedToGold message, IMessageHandlerContext context)
        {
            Console.WriteLine("Notify the customer about gold status");
            return Task.CompletedTask;
        }
    }
}
