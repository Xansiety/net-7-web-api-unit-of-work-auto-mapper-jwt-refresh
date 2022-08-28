using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly TiendaContext _context;
    public GenericRepository(TiendaContext context)
    {
        _context = context;
    }


    public virtual void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public virtual Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public virtual void Remove(T entity)
    {
        throw new NotImplementedException();
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public virtual void Update(T entity)
    {
        throw new NotImplementedException();
    }
}

