namespace API.Helpers.Errors;
public class APIValidation: APIResponse
{
    public APIValidation():base(400)
    {

    }

    public IEnumerable<string> Errors { get; set; }

}
