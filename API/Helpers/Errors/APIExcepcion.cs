namespace API.Helpers.Errors;

public class APIExcepcion : APIResponse
{
    public APIExcepcion(int statusCode, string message = null, string details = null) : base(statusCode, message)
    {
        Details = details;
    }
    public string Details { get; set; }

}
