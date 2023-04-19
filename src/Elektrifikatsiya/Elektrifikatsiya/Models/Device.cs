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
    public bool Available { get; set; }

    public Device(string macAddress, string name, IPAddress address, User user, string room)
    {
        MacAddress = macAddress;
        Name = name;
        IpAddress = address;
        User = user;
        Room = room;
    }

    private Device(){}
}