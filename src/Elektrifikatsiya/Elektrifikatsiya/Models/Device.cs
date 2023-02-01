using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Elektrifikatsiya.Model;

public class Device
{
    [Key]
    public string Key { get; private set; }
    [Required]
    public string Name { get; private set; } = string.Empty;
    [Required]
    public IPAddress Address { get; private set; }

    public Device(string key, IPAddress address)
    {
        Key = key;
        Address = address;
    }
}