using Blazorise.Charts;

using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using System.Diagnostics;
using System.Text;

namespace Elektrifikatsiya.Pages;

public partial class Dashboard
{
    //TODO: insert new event here if plug produces one
    private readonly List<Event> events = new List<Event>() { new Event("Placeholder", "Placeholder", DateTime.Now), new Event("Placeholder", "Placeholder", DateTime.Now), new Event("Placeholder", "Placeholder", DateTime.Now) };

    //TODO: insert new Device here if user adds one
    private List<Device> plugs = new List<Device>();

    //code for graph
    private LineChart<double> lineChart;

    protected override void OnInitialized()
    {
        plugs = DeviceStatusService.GetDevices().ValueOrDefault ?? new List<Device>();

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

    private async Task Switch(Device device)
    {
        device.Enabled = !device.Enabled;
        _ = await DeviceManagmentService.UpdateDevice(device);
    }

    private async Task HandleRedraw()
    {
        labels.Clear();
        await lineChart.Clear();
        await lineChart.AddLabelsDatasetsAndUpdate(labels, await GetLineChartDataset());
    }

    //TODO: insert dataset of current and last voltage usages
    private async Task<LineChartDataset<double>> GetLineChartDataset()
    {
        return new LineChartDataset<double>
        {
            Label = "Wattage",
            Data = await DeviceData(),
            Fill = true,
            PointRadius = 3,
            CubicInterpolationMode = "monotone",
        };
    }

    private readonly List<string> labels = new List<string>();

    private async Task<List<double>> DeviceData()
    {
        PrometheusQuery promQueryer = new PrometheusQuery("http://localhost:9090");

        StringBuilder builder = new();
        foreach (Device device in plugs)
        {
            _ = builder.Append($"shellyplug-s-{device.MacAddress}/relay/0|");
        }

        if (builder.Length <= 1)
        {
            return new();
        }

        string plugnames = builder.ToString()[..^1];

        Debug.WriteLine($$"""sum(power{sensor=~"{{plugnames}}"})[1h:1m]""");

        PrometheusDataWrapper? deviceData = (await promQueryer.Query($$"""sum(power{sensor=~"{{plugnames}}"})[1h:1m]"""))?.Data;

        Random r = new Random(DateTime.Now.Millisecond);

        return deviceData?.MatrixTypeToTimestampFloatTuple().ValueOrDefault?.Select(x =>
        {
            labels.Add(DateTimeOffset.FromUnixTimeSeconds(long.Parse($"1{x.Item1}0")).UtcDateTime.ToLocalTime().ToShortTimeString());
            return x.Item2;
        }).ToList() ?? new List<double>();
    }
}