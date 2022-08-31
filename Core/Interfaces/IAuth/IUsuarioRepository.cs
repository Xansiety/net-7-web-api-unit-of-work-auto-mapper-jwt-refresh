using Core.Entities.Auth; 
namespace Core.Interfaces.IAuth;  
public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario> GetByUserNameAsync(string username);
    Task<Usuario> GetByRefreshTokenAsync(string refreshToken);
}
