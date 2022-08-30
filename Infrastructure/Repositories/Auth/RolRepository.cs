using Core.Entities.Auth;
using Core.Interfaces.IAuth;
using Infrastructure.Data;

namespace Infrastructure.Repositories.Auth;

public class RolRepository : GenericRepository<Rol>, IRolRepository
{
    public RolRepository(TiendaContext context) : base(context)
    {
    }
}

