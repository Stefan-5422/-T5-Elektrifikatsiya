namespace Elektrifikatsiya.Models;

public class ShellyResponse
{
    public string Type { get; set; }
    public string Mac { get; set; }

    public ShellyResponse(string type, string mac)
    {
        Type = type;
        Mac = mac;
    }
}