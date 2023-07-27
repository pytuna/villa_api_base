using System.Net;

namespace VillaApi.Models;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = false;
    public List<string>? Errors { get; set; }
    public object? Data { get; set; }

    public void PushErrors(string error)
    {
        if (Errors == null)
        {
            Errors = new();
        }
        Errors.Add(error);
    }

    public ApiResponse Success(object? _data = null, HttpStatusCode _statusCode = HttpStatusCode.OK)
    {
        StatusCode = _statusCode;
        IsSuccess = true;
        Data = _data;
        return this;
    }

    public ApiResponse Fail(HttpStatusCode _statusCode, string[]? _error = null)
    {
        StatusCode = _statusCode;
        IsSuccess = false;
        if(_error != null)
        {
            Errors = _error.ToList();
        }
        return this;
    }
}