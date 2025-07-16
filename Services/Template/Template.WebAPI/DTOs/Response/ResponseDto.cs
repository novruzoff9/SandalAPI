namespace Template.WebAPI.DTOs.Response;

public class ResponseDto<T>
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public IReadOnlyList<string> Errors { get; set; }
}
