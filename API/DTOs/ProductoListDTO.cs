﻿namespace API.DTOs;

public class ProductoListDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int MarcaId { get; set; }
    public string Marca { get; set; }
    public int CategoriaId { get; set; }
    public string Categoria { get; set; }

    // En respuestas complejas de XML de debe de añadir este constructor por lo cual se agrega, si solo de devuelve JSON no es necesario

    public ProductoListDTO()
    {

    }
}
