using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Elektrifikatsiya.Models;

public class Device
{
    [Key]
    public string Key { get; private set; }

    [Required]
    public string Name { get; private set; }

    [Required]
    public IPAddress Address { get; private set; }

    public int PowerUsage { get; set; }
    public string User { get; set; }
    public string Room { get; set; }

    public Device(string key, string name, IPAddress address, int powerUsage, string user, string room)
    {
        Key = key;
        Name = name;
        Address = address;
        PowerUsage = powerUsage;
        User = user;
        Room = room;
    }
}