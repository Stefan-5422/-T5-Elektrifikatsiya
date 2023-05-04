using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Server.IIS.Core;

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
    public bool Available { get; set; }

    public Device(string macAddress, string name, IPAddress address, User user, string room)
    {
        MacAddress = macAddress;
        Name = name;
        IpAddress = address;
        User = user;
        Room = room;
    }

    public static Device CopyDevice(Device device) => new Device(device.MacAddress, device.Name, device.IpAddress, device.User, device.Room);

    public void OverwiteDevice(Device overwriter)
    {
        this.Room = overwriter.Room;
        this.Available = overwriter.Available;
        this.PowerUsage = overwriter.PowerUsage;
        this.Name = overwriter.Name;
        this.User = overwriter.User;
        this.MacAddress = overwriter.MacAddress;
        this.IpAddress = overwriter.IpAddress;
    }
    private Device(){}
}