using Blazorise.Charts;

using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using System.Diagnostics;
using System.Text;

namespace Elektrifikatsiya.Pages;

public partial class Dashboard
{
	//TODO: insert new event here if plug produces one
	List<Event> events = new List<Event>() { new Event("Text", "Text", DateTime.Today, null ) };
	//TODO: insert new Device here if user adds one
	List<Device> plugs = new List<Device>();
	//code for graph
	LineChart<double> lineChart;

	protected override void OnInitialized()
	{
		plugs = DeviceStatusService.GetDevices().ValueOrDefault ?? new List<Device>();
		events = EventService.Events;

		EventService.OnEventCalled += (_, e) =>
		{
			_ = InvokeAsync(StateHasChanged);
		};
		DeviceStatusService.OnDeviceStatusChanged += (_, e) =>
		{
			_ = InvokeAsync(StateHasChanged);
		};
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await HandleRedraw();
		}
	}

	async Task HandleRedraw()
	{
		labels.Clear();
		await lineChart.Clear();
		await lineChart.AddLabelsDatasetsAndUpdate(labels, await GetLineChartDataset());
	}

	//TODO: insert dataset of current and last voltage usages
	async Task<LineChartDataset<double>> GetLineChartDataset()
	{
		return new LineChartDataset<double>
		{
			Label = "Wattage",
			Data = await RandomizeData(),
			Fill = true,
			PointRadius = 3,
			CubicInterpolationMode = "monotone",
		};
	}

	List<string> labels = new List<string>();

	async Task<List<double>> RandomizeData()
	{
		if (plugs.Count == 0)
		{
			return new();
		}
		PrometheusQuery promQueryer = new PrometheusQuery("http://localhost:9090");

		string plugnames = "";
		foreach (Device device in plugs)
		{
			plugnames += $"shellyplug-s-{device.MacAddress}/relay/0|";
		}
		plugnames = plugnames[0..^1];

		Debug.WriteLine($$"""sum(power{sensor=~"{{plugnames}}"})[1h:1m]""");

		PrometheusDataWrapper? deviceData = (await promQueryer.Query($$"""sum(power{sensor=~"{{plugnames}}"})[1h:1m]"""))?.Data;

		var r = new Random(DateTime.Now.Millisecond);

		return deviceData?.MatrixTypeToTimestampFloatTuple().ValueOrDefault?.Select(x =>
		{
			labels.Add(DateTimeOffset.FromUnixTimeSeconds(long.Parse($"1{x.Item1}0")).UtcDateTime.ToLocalTime().ToShortTimeString());
			return x.Item2;
		}).ToList() ?? new List<double>();
	}
}
