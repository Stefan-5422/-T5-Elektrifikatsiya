using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elektrifikatsiya.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; private set; }

    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public string? SessionToken { get; set; }
    public DateTime LastLoginDate { get; set; }
    public Role Role { get; set; }

    public User(string name, string passwordHash, Role role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Role = role;
    }
}