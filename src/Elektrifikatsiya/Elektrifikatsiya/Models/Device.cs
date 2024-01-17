using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Elektrifikatsiya.Models;

public class Device
{
    [Key]
    public string MacAddress { get; private set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public IPAddress IpAddress { get; set; }

    [Required]
    public User User { get; set; }

    [NotMapped]
    public double PowerUsage { get; set; }

    public string Room { get; set; }

    [NotMapped]
    public bool Enabled { get; set; }

    public Device(string macAddress, string name, IPAddress address, User user, string room)
    {
        MacAddress = macAddress;
        Name = name;
        IpAddress = address;
        User = user;
        Room = room;
    }

    public Device CopyDevice()
    {
        return new Device(MacAddress, Name, IpAddress, User, Room);
    }

    public void OverwiteDevice(Device overwriter)
    {
        Room = overwriter.Room;
        Enabled = overwriter.Enabled;
        PowerUsage = overwriter.PowerUsage;
        Name = overwriter.Name;
        User = overwriter.User;
        MacAddress = overwriter.MacAddress;
        IpAddress = overwriter.IpAddress;
    }

    private Device()
    { }
}