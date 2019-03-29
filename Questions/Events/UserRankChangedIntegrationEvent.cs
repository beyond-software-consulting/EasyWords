using System;
using EventBus.Events;

namespace Questions.EventHandlers
{
    public class UserRankChangedIntegrationEvent:IntegrationEvent
    {
        public UserRankChangedIntegrationEvent(Guid userID,float userRank)
        {
            UserID = userID;
            UserRank = userRank;
        }

        public Guid UserID { get; set; }
        public float UserRank { get; set; }
    }
}
