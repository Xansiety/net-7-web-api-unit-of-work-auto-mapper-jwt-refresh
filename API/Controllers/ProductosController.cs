using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ProductosController : BaseApiController
    {

        private readonly IUnityOfWork _unityOfWork;

        public ProductosController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Producto>>> Get()
        {
            var productos = await _unityOfWork.Productos.GetAllAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Producto>>> Get(int id)
        {
            var productos = await _unityOfWork.Productos.GetByIdAsync(id);
            return Ok(productos);
        }
    }
}
