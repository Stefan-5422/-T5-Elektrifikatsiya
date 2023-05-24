namespace Elektrifikatsiya.Models
{
	public class EventServiceEventArgs : EventArgs
	{
		public Event NewEvent { get; set; } = new Event();
		public EventServiceEventArgs(Event newEvent)
		{
			NewEvent = newEvent;
		}
	}
}
