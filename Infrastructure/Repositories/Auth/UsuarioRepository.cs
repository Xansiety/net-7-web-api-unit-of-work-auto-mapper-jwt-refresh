using Core.Entities.Auth;
using Core.Interfaces.IAuth;
using Infrastructure.Data;
namespace Infrastructure.Repositories.Auth;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(TiendaContext context) : base(context)
    {
    }
}


