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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Producto>> Post(Producto producto)
        {
            _unityOfWork.Productos.Add(producto);
            _unityOfWork.Save();
            if (producto is null) return BadRequest();

            return CreatedAtAction(nameof(Post), new { id = producto.Id }, producto);
        }


        //PUT: api/productos/28
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Producto>> Put(int id, [FromBody] Producto producto)
        {
            if (producto is null) return NotFound();

            _unityOfWork.Productos.Update(producto);
            _unityOfWork.Save();
            if (producto is null) return BadRequest();

            return producto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _unityOfWork.Productos.GetByIdAsync(id);
            if (producto is null) return NotFound();

            _unityOfWork.Productos.Remove(producto);
            _unityOfWork.Save();

            return NoContent();

        }
    }
}
