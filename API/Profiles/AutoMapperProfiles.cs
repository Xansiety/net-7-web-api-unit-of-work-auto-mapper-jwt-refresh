using API.DTOs;
using AutoMapper;
using Core.Entities;

namespace API.Profiles;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //map producto
        CreateMap<Producto, ProductoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Marca, MarcaDTO>().ReverseMap();

        CreateMap<Producto, ProductoListDTO>()
            .ForMember(destino => destino.Marca, origen => origen.MapFrom(origen => origen.Marca.Nombre))
            .ForMember(destino => destino.Categoria, origen => origen.MapFrom(origen => origen.Categoria.Nombre))
            .ReverseMap()
            .ForMember(origen => origen.Categoria, destino => destino.Ignore())
            .ForMember(origen => origen.Marca, destino => destino.Ignore());

        CreateMap<Producto, PostPutProductoDTO>() 
             .ReverseMap()
             .ForMember(origen => origen.Categoria, destino => destino.Ignore())
             .ForMember(origen => origen.Marca, destino => destino.Ignore());
    }
}
