using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/productos")]
    [ApiController]
    //indicar que el controlador soporta las siguientes versiones
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    //Authorization
    [Authorize]
    public class ProductosController : BaseApiController
    {

        private readonly IUnityOfWork _unityOfWork;
        private readonly IMapper _mapper;

        public ProductosController(IUnityOfWork unityOfWork, IMapper mapper)
        {
            _unityOfWork = unityOfWork;
            _mapper = mapper;
        }


        //[HttpGet]      
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<IEnumerable<ProductoListDTO>>> Get()
        //{
        //    var productos = await _unityOfWork.Productos.GetAllAsync();
        //    return Ok(_mapper.Map<List<ProductoListDTO>>(productos));
        //}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Paginator<ProductoListDTO>>> Get([FromQuery] Params productParams)
        {
            var resultado = await _unityOfWork.Productos
                                        .GetAllAsync(productParams.PageIndex, productParams.PageSize,
                                        productParams.Search);

            var listaProductosDto = _mapper.Map<List<ProductoListDTO>>(resultado.registros);

            Response.Headers.Add("X-Total-registros", resultado.totalRegistros.ToString());

            return new Paginator<ProductoListDTO>(listaProductosDto, resultado.totalRegistros,
                productParams.PageIndex, productParams.PageSize, productParams.Search);

        }


        [HttpGet]
        [MapToApiVersion("1.1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> Get1()
        {
            var productos = await _unityOfWork.Productos.GetAllAsync();
            return Ok(_mapper.Map<List<ProductoDTO>>(productos));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDTO>> Get(int id)
        {
            var producto = await _unityOfWork.Productos.GetByIdAsync(id); 
            if (producto is null) return NotFound();  
            return Ok(_mapper.Map<ProductoDTO>(producto));
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostPutProductoDTO>> Post(PostPutProductoDTO productDTO)
        {
            var producto = _mapper.Map<Producto>(productDTO);
            _unityOfWork.Productos.Add(producto);
            await _unityOfWork.SaveAsync();
            if (producto is null) return BadRequest();
            productDTO.Id = producto.Id;
            return CreatedAtAction(nameof(Post), new { id = productDTO.Id }, productDTO);
        }


        //PUT: api/productos/28
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostPutProductoDTO>> Put(int id, [FromBody] PostPutProductoDTO productoDTO)
        {
            if (productoDTO is null) return NotFound();

            var producto = _mapper.Map<Producto>(productoDTO); 
            _unityOfWork.Productos.Update(producto);
            await _unityOfWork.SaveAsync(); 
            return productoDTO;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _unityOfWork.Productos.GetByIdAsync(id);
            if (producto is null) return NotFound();

            _unityOfWork.Productos.Remove(producto);
            await _unityOfWork.SaveAsync();

            return NoContent();

        }
    }
}
