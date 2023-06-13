using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

namespace Elektrifikatsiya.Services;

public interface IEnergyPriceService : IScheduledService
{
    public double DailyEnergyCost { get; }
    public double MonthlyEnergyCost { get; }
    public double YearlyEnergyCost { get; }

    public Task<double> GetEnergyPrice(DateTime startime, DateTime endTime, List<Device> plugs, List<EnergyPriceChange> changes);
}