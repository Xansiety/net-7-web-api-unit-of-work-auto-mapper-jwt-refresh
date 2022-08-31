using System.ComponentModel.DataAnnotations;
namespace API.DTOs.AuthDTO;
public class LoginDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
