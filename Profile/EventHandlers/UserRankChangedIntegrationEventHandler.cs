using System;
using System.Threading.Tasks;
using EventBus.Interfaces;

namespace Profile.EventHandlers
{
    public class UserRankChangedIntegrationEventHandler:IIntegrationEventHandler<UserRankChangedIntegrationEvent>
    {
        public UserRankChangedIntegrationEventHandler()
        {
        }

        public Task Handle(UserRankChangedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
