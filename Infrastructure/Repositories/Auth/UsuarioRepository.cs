using Core.Entities.Auth;
using Core.Interfaces.IAuth;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Auth;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(TiendaContext context) : base(context)
    {
    }

    public async Task<Usuario> GetByUserNameAsync(string username)
    {
        return await _context.Usuarios
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
    }
}


