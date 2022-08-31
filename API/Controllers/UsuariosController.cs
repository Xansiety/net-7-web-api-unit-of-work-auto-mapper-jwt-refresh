using API.Dtos;
using API.DTOs.AuthDTO;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsuariosController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterDTO model)
        {
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }


        [HttpPost("token")]
        public async Task<ActionResult> RegisterAsync(LoginDTO model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }


        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleDTO model)
        {
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }

    }
}
