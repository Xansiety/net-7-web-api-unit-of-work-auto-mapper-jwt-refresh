using API.Dtos;
using API.Helpers;
using Core.Entities.Auth;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
namespace API.Services;
public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnityOfWork _unitOfWork;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UserService(IUnityOfWork unitOfWork, IOptions<JWT> jwt,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> RegisterAsync(RegisterDTO registerDto)
    {
        //Asignación información del usuario
        var usuario = new Usuario
        {
            Nombres = registerDto.Nombres,
            ApellidoMaterno = registerDto.ApellidoMaterno,
            ApellidoPaterno = registerDto.ApellidoPaterno,
            Email = registerDto.Email,
            UserName = registerDto.Username
        };
        //encriptar password
        usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password);
        //Validar Usuario existe username en DB
        var usuarioExiste = _unitOfWork.Usuarios
                                    .Find(u => u.UserName.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            //buscamos el rol predeterminado
            var rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => u.Nombre == Autorizacion.rol_predeterminado.ToString())
                                    .First();
            try
            {
                //agregamos el rol de la colección
                usuario.Roles.Add(rolPredeterminado);
                _unitOfWork.Usuarios.Add(usuario);
                await _unitOfWork.SaveAsync();

                return $"El usuario {registerDto.Username} ha sido registrado exitosamente";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"El usuario con {registerDto.Username} ya se encuentra registrado.";
        }
    }

}
