using API.Dtos;
using API.DTOs.AuthDTO;

namespace API.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDTO registerDTO);
    Task<DatosUsuarioDTO> GetTokenAsync(LoginDTO loginDTO);
    Task<string> AddRoleAsync(AddRoleDTO model);
    Task<DatosUsuarioDTO> RefreshTokenAsync(string refreshToken);
}
