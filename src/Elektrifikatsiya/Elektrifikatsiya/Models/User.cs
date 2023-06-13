using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elektrifikatsiya.Models;

public class User
{
	[Required]
	public List<Device> Devices { get; set; } = new();

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Key]
	public int Id { get; private set; }

	public DateTime LastLoginDate { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string PasswordHash { get; private set; }

	[Required]
	public string Email { get; set; }

	[Required]
	public Role Role { get; set; }


	public string? SessionToken { get; set; }

	public User(string name, string passwordHash, string email, Role role)
	{
		Name = name;
		PasswordHash = passwordHash;
		Email = email;
		Role = role;
	}
}