using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elektrifikatsiya.Models
{
	public class Event
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int Id { get; private set; }
		public string EventName { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public Device Plug { get; set; }

		public Event() { }
		public Event(string eventName, string description, DateTime date, Device plug)
		{
			EventName = eventName;
			Description = description;
			Date = date;
			Plug = plug;
		}
	}
}
