using API.Dtos;
using API.DTOs.AuthDTO;
using API.Helpers;
using Core.Entities;
using Core.Entities.Auth;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

    public async Task<DatosUsuarioDTO> GetTokenAsync(LoginDTO model)
    {
        DatosUsuarioDTO datosUsuarioDto = new DatosUsuarioDTO();
        //validar el usuario y sus roles
        var usuario = await _unitOfWork.Usuarios.GetByUserNameAsync(model.Username);

        if (usuario == null)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"No existe ningún usuario con el username {model.Username}.";
            return datosUsuarioDto;
        }

        //si existe el usuario, validamos el password
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);

        //si la validación es correcta asignamos la información
        if (resultado == PasswordVerificationResult.Success)
        {
            datosUsuarioDto.EstaAutenticado = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario); //creamos el token
            datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            datosUsuarioDto.Email = usuario.Email;
            datosUsuarioDto.UserName = usuario.UserName;
            datosUsuarioDto.Roles = usuario.Roles
                                            .Select(u => u.Nombre)
                                            .ToList(); //arreglo de roles


            //lógica para refresh token

            //si existe algún RefreshToken activo regreso el token
            if (usuario.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = usuario.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                datosUsuarioDto.RefreshToken = activeRefreshToken.Token;
                datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                //se crea un nuevo token
                var refreshToken = CreateRefreshToken();
                datosUsuarioDto.RefreshToken = refreshToken.Token;
                datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
                usuario.RefreshTokens.Add(refreshToken);
                _unitOfWork.Usuarios.Update(usuario);
                await _unitOfWork.SaveAsync();
            }
            return datosUsuarioDto;
        }
        //si no esta autenticado
        datosUsuarioDto.EstaAutenticado = false;
        datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuario.UserName}.";
        return datosUsuarioDto;
    }

    private JwtSecurityToken CreateJwtToken(Usuario usuario)
    {
        //añadimos los roles en una lista de claims
        var roles = usuario.Roles;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Nombre));
        }
        var claims = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
          new Claim("uid", usuario.Id.ToString())
        }
        .Union(roleClaims); //unimos los roles

        //generamos el token de identificación con los datos del appSettings 
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwt.DurationInMinutes)),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }


    private RefreshToken CreateRefreshToken()
    {
        var randonNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randonNumber);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randonNumber),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow        
        };
    }

    public async Task<DatosUsuarioDTO> RefreshTokenAsync(string refreshToken)
    {
        var datosUsuarioDto = new DatosUsuarioDTO();

        var usuario = await _unitOfWork.Usuarios
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"El token no pertenece a ningún usuario.";
            return datosUsuarioDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje = $"El token no está activo.";
            return datosUsuarioDto;
        }
        //Revocamos el Refresh Token actual y
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generamos un nuevo Refresh Token y lo guardamos en la Base de Datos
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveAsync();
        //Generamos un nuevo Json Web Token 😊
        datosUsuarioDto.EstaAutenticado = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        datosUsuarioDto.Email = usuario.Email;
        datosUsuarioDto.UserName = usuario.UserName;
        datosUsuarioDto.Roles = usuario.Roles
                                        .Select(u => u.Nombre)
                                        .ToList();
        datosUsuarioDto.RefreshToken = newRefreshToken.Token;
        datosUsuarioDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return datosUsuarioDto;
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

    public async Task<string> AddRoleAsync(AddRoleDTO model)
    {

        var usuario = await _unitOfWork.Usuarios
                    .GetByUserNameAsync(model.Username);

        if (usuario == null)
        {
            return $"No existe algún usuario registrado con la cuenta {model.Username}.";
        }


        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);

        if (resultado == PasswordVerificationResult.Success)
        {


            var rolExiste = _unitOfWork.Roles
                                        .Find(u => u.Nombre.ToLower() == model.Role.ToLower())
                                        .FirstOrDefault();

            if (rolExiste != null)
            {
                var usuarioTieneRol = usuario.Roles
                                            .Any(u => u.Id == rolExiste.Id);

                if (usuarioTieneRol == false)
                {
                    usuario.Roles.Add(rolExiste);
                    _unitOfWork.Usuarios.Update(usuario);
                    await _unitOfWork.SaveAsync();
                }

                return $"Rol {model.Role} agregado a la cuenta {model.Username} de forma exitosa.";
            }

            return $"Rol {model.Role} no encontrado.";
        }
        return $"Credenciales incorrectas para el usuario {usuario.UserName}.";
    }

}
