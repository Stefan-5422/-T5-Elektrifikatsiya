using Elektrifikatsiya.Models;

namespace Elektrifikatsiya.Services
{
    public interface IEventService
    {
        public List<Event> Events { get; }
        public event EventHandler<EventServiceEventArgs> OnEventCalled;
    }
}
