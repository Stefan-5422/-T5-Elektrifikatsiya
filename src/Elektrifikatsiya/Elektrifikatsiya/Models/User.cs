using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elektrifikatsiya.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; private set; }

    [Required]
    public string Name { get; private set; }

    [Required]
    public string PasswordHash { get; private set; }

    [Required]
    public Role Role { get; set; }

    public string? SessionToken { get; set; }
    public DateTime LastLoginDate { get; set; }

    [Required]
    public List<Device> Devices { get; set; } = new();

    public User(string name, string passwordHash, Role role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Role = role;
    }
}