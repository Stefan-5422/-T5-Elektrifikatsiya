using System.ComponentModel.DataAnnotations;

namespace Elektrifikatsiya.Models;

public class EnergyPriceChange
{
	[Key]
	public DateTime DateTime { get; set; }
    public double EnergyPrice { get; set; }


    public EnergyPriceChange(DateTime dateTime, double energyPrice)
	{
		DateTime = dateTime;
		EnergyPrice = energyPrice;
	}

	public EnergyPriceChange(double energyPrice)
	{
		EnergyPrice = energyPrice;
		DateTime = DateTime.Now;
	}
}