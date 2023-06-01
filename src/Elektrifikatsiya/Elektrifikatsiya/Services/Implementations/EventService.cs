using Elektrifikatsiya.Models;
using FluentResults;
using System.Reflection;

namespace Elektrifikatsiya.Services.Implementations
{
	public class EventService : IEventService
	{
		private readonly ILogger<EventService> logger;
		public event EventHandler<EventServiceEventArgs> OnEventCalled;
		public List<Event> Events { get; } = new();

		public EventService(ILogger<EventService> logger, IDeviceStatusService deviceStatusService, IDeviceManagmentService deviceManagmentService)
		{
			this.logger = logger;
			List<Device> deviceList = deviceManagmentService.GetDevices().ValueOrDefault.Select(x=>x.CopyDevice()).ToList();

			deviceStatusService.OnDeviceStatusChanged += (_, e) =>
			{
				bool newDevice = true;
				Device currentDevice = deviceManagmentService.GetDevice(e.MacAddress).ValueOrDefault;

				for (int i = 0; i < deviceList.Count; i++)
				{
					if (deviceList[i].MacAddress == currentDevice.MacAddress)
					{
						PropertyInfo[] properties = typeof(Device).GetProperties();

						foreach(PropertyInfo property in properties) { 
							var currentvalue = property.GetValue(deviceList[i], null);
							var newvalue = property.GetValue(currentDevice, null);
							if(newvalue is not null && !newvalue.ToString().Equals(currentvalue.ToString()))
							{
								string eventName = $"Plug {deviceList[i].Name} changed: {property.Name}";
								string description = $"Changed {property.Name} of plug [{currentvalue}] to [{newvalue}]";
								DateTime dateTime = DateTime.Now;

								Event newEvent = new Event(eventName, description, dateTime, currentDevice);
								Events.Insert(0,newEvent);
								OnEventCalled?.Invoke(this, new EventServiceEventArgs(newEvent));
							}

						}
						deviceList[i].OverwiteDevice(currentDevice);
						newDevice = false; break;
					}
				}
				if (newDevice)
				{
					deviceList.Add(deviceManagmentService.GetDevice(e.MacAddress).ValueOrDefault);
				}
			};
		}
	}
}
