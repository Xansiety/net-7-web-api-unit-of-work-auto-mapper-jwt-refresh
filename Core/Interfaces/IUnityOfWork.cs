namespace Core.Interfaces;

public interface IUnityOfWork
{
    IProductoRepository Productos { get; }
    IMarcaRepository  Marcas { get; }
    ICategoriaRepository Categorias { get;  }
    int Save();
}
