namespace Core.Interfaces;

public interface IUnityOfWork
{
    IProductoRepository Productos { get; }
    IMarcaRepository  Marcas { get; }
    ICategoriaRepository Categorias { get;  }
    Task<int> SaveAsync();
}
