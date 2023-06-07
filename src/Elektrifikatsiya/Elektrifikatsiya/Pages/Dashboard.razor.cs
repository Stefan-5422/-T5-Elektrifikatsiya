using Blazorise.Charts;

using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using System.Diagnostics;
using System.Text;
using Elektrifikatsiya.Services.Implementations;
using Elektrifikatsiya.Database;

namespace Elektrifikatsiya.Pages;

public partial class Dashboard
{
    //TODO: insert new event here if plug produces one
    private List<Event> events = new List<Event>() { new Event("Text", "Text", DateTime.Today, null) };

    private List<string> labels = new List<string>();

    //code for graph
    private LineChart<double> lineChart;

    //TODO: insert new Device here if user adds one
    private List<Device> plugs = new List<Device>();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await HandleRedraw();
        }
    }

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

    private async Task<List<double>> GetData()
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

        double energyPrice = MainDatabaseContext.EnergyPriceChanges.OrderByDescending(e =>e.DateTime).FirstOrDefault()?.EnergyPrice ?? 0;

        PrometheusDataWrapper? deviceData = (await promQueryer.Query($$"""sum(power{sensor=~"{{plugnames}}"})[1h:1m]"""))?.Data;
        //PrometheusDataWrapper? priceData = (await promQueryer.Query($$"""sum(sum_over_time(power{sensor=~"{{plugnames}}"}[1y])*{{energyPrice}})"""))?.Data;

        //Console.WriteLine(priceData.VectorTypeToTimestampFloatTuple());

        var r = new Random(DateTime.Now.Millisecond);

        return deviceData?.MatrixTypeToTimestampFloatTuple().ValueOrDefault?.Select(x =>
        {
            labels.Add(DateTimeOffset.FromUnixTimeSeconds(long.Parse($"1{x.Item1}0")).UtcDateTime.ToLocalTime().ToShortTimeString());
            return x.Item2;
        }).ToList() ?? new List<double>();
    }

    //TODO: insert dataset of current and last voltage usages
    private async Task<LineChartDataset<double>> GetLineChartDataset()
    {
        return new LineChartDataset<double>
        {
            Label = "Wattage",
            Data = await GetData(),
            Fill = true,
            PointRadius = 3,
            CubicInterpolationMode = "monotone",
        };
    }

    private async Task HandleRedraw()
    {
        labels.Clear();
        await lineChart.Clear();
        await lineChart.AddLabelsDatasetsAndUpdate(labels, await GetLineChartDataset());
    }

    private async Task Switch(Device device)
    {
        device.Enabled = !device.Enabled;
        _ = await DeviceManagmentService.UpdateDevice(device);
    }
}