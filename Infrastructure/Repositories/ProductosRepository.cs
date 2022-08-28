using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductosRepository : GenericRepository<Producto>, IProductosRepository
    {
        public ProductosRepository(TiendaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetProductosMasCaros(int cantidad) =>
            await _context.Productos
            .OrderByDescending(p => p.Precio)
            .Take(cantidad)
            .ToListAsync();


    }
}
