using System;
using System.Threading.Tasks;
using EventBus.Interfaces;
using Questions.EventHandlers;

namespace Questions.Events
{
    public class UserRankChangedIntegrationEventHandler : IIntegrationEventHandler<UserRankChangedIntegrationEvent>
    {
        public UserRankChangedIntegrationEventHandler()
        {
        }

        public async Task Handle(UserRankChangedIntegrationEvent @event)
        {
            Console.WriteLine("Event Received from UserID:{0}",@event.UserID);
        }
    }
}
