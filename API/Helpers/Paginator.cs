namespace API.Helpers;

public class Paginator<T> where T : class
{

    // Esto es totalmente valido si solo se devuelve JSON como respuesta
    public string Search { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int Total { get; private set; }
    public IEnumerable<T> Registers { get; private set; }


    // esto se agrega para que se pueda serializar a XML en objetos complejos
    //public string Search { get; set; }
    //public int PageIndex { get; set; }
    //public int PageSize { get; set; }
    //public int Total { get; set; }
    //public List<T> Registers { get; set; }

    //// En respuestas complejas de XML de debe de añadir este constructor por lo cual se agrega, si solo de devuelve JSON no es necesario
    //public Paginator()
    //{

    //}

    //public Paginator(List<T> registers, int total, int pageIndex,
    public Paginator(IEnumerable<T> registers, int total, int pageIndex,
        int pageSize, string search)
    {
        Registers = registers;
        Total = total;
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }

    public int TotalPages
    {
        get
        {

            return (int)Math.Ceiling(Total / (double)PageSize);
        }
        // esto se agrega para que se pueda serializar a XML en objetos complejos
        //set
        //{
        //    this.TotalPages = value;
        //}
    }

    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 1);
        }
        // esto se agrega para que se pueda serializar a XML en objetos complejos
        //set
        //{
        //    this.HasPreviousPage = value;
        //}
    }

    public bool HasNextPage
    {
        get
        {
            return (PageIndex < TotalPages);
        }
        // esto se agrega para que se pueda serializar a XML en objetos complejos
        //set
        //{
        //    this.HasNextPage = value;
        //}
    }
}
