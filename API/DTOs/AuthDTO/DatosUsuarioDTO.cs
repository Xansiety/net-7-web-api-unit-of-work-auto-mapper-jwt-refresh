using System.Text.Json.Serialization;

namespace API.Dtos;
public class DatosUsuarioDTO
{
    public string Mensaje { get; set; }
    public bool EstaAutenticado { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    [JsonIgnore] //restringe que esta propiedad sea mostrada en la respuesta de json
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration {get; set;}
}
