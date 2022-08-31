using Core.Interfaces.IAuth;

namespace Core.Interfaces;

public interface IUnityOfWork
{
    IProductoRepository Productos { get; }
    IMarcaRepository  Marcas { get; }
    ICategoriaRepository Categorias { get;  }
    IRolRepository Roles { get; set; }
    IUsuarioRepository Usuarios { get; set; }
    Task<int> SaveAsync();
}
