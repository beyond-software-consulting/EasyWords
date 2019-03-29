using System;
using System.Threading.Tasks;

namespace EventBus.Interfaces
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>:IIntegratoinEventHandler 
        where TIntegrationEvent: Events.IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegratoinEventHandler
    {
    }
}
