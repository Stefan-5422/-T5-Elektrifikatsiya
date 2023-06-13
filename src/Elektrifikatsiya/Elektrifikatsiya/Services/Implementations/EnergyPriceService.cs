using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using System.Globalization;

namespace Elektrifikatsiya.Services.Implementations;

public class EnergyPriceService : IEnergyPriceService
{
	private readonly MainDatabaseContext mainDatabaseContext;
	private readonly PrometheusQuery prometheusQuerier = new("http://127.0.0.1:9090");

	public TimeSpan ExecutionRepeatDelay { get; } = TimeSpan.FromSeconds(5);
	public DateTime FirstExecutionTime { get; } = DateTime.Now.AddSeconds(5);
	public double MonthlyEnergyCost { get; private set; }

	public double YearlyEnergyCost { get; private set; }

	public double DailyEnergyCost { get; private set; }

	public EnergyPriceService(IServiceScopeFactory serviceScopeFactory)
	{
		mainDatabaseContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDatabaseContext>();
	}

	/// <summary>
	/// Calculate the energy price for the given timespan
	/// </summary>
	/// <param name="startime">The timestamp that has the greater value</param>
	/// <param name="endTime">The timestamp that has the lower value</param>
	/// <param name="plugs"></param>
	/// <param name="changes"></param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public async Task<double> GetEnergyPrice(DateTime startime, DateTime endTime, List<Device> plugs, List<EnergyPriceChange> changes)
	{
		string plugnames = "";
		foreach (Device device in plugs)
		{
			plugnames += $"shellyplug-s-{device.MacAddress}/relay/0|";
		}
		plugnames = plugnames[0..^1];

		if (startime < endTime)
		{
			throw new ArgumentException("Read the xmlDoc");
		}

		changes = changes.OrderByDescending(x => x.DateTime).ToList();

		double total = (await prometheusQuerier.Query($$"""sum_over_time(sum(power{sensor=~"{{plugnames}}"})[{{(int)Math.Ceiling((startime - changes.First().DateTime).TotalMinutes)}}m:1s] @ {{((DateTimeOffset)startime).ToUnixTimeSeconds()}}) * {{(changes.First().EnergyPrice / 3600).ToString(CultureInfo.InvariantCulture)}}"""))?.Data?.VectorTypeToTimestampFloatTuple().ValueOrDefault.Item2 ?? 0;

		for (int i = 0; i < changes.Count - 1; i++)
		{
			bool inMinutes = true;
			int duration = (int)Math.Ceiling((changes[i].DateTime - changes[i + 1].DateTime).TotalMinutes);
			if (duration > 100000)
			{
				inMinutes = false;
				duration = (int)Math.Ceiling(duration / 1440.0);
			}
			long timestamp = ((DateTimeOffset)changes[i].DateTime).ToUnixTimeSeconds();

			double price = changes[i + 1].EnergyPrice;

			total += (await prometheusQuerier.Query($$"""sum_over_time(sum(power{sensor=~"{{plugnames}}"})[{{duration}}{{(inMinutes ? "m" : "d")}}:1s] @ {{timestamp}}) * {{(price / 3600).ToString(CultureInfo.InvariantCulture)}}"""))?.Data?.VectorTypeToTimestampFloatTuple().ValueOrDefault.Item2 ?? 0;
			if (changes[i].DateTime < endTime)
			{
				break;
			}
		}

		return total;
	}

	public async void Update()
	{
		List<Device> plugs = mainDatabaseContext.Devices.ToList();
		List<EnergyPriceChange> energyPriceChanges = mainDatabaseContext.EnergyPriceChanges.ToList();

		DailyEnergyCost = await GetEnergyPrice(DateTime.Now, DateTime.Now.AddDays(-1), plugs, energyPriceChanges);
		MonthlyEnergyCost = await GetEnergyPrice(DateTime.Now, DateTime.Now.AddMonths(-1), plugs, energyPriceChanges);
		YearlyEnergyCost = await GetEnergyPrice(DateTime.Now, DateTime.Now.AddYears(-1), plugs, energyPriceChanges);
	}
}