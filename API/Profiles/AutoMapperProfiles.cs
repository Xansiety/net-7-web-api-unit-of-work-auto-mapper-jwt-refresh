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


    }
}
