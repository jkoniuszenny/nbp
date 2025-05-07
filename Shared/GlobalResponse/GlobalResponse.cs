namespace Shared.GlobalResponse;

public class GlobalResponse : IGlobalResponse
{
    public bool Succeded { get; protected set; }
    public Error Error { get; protected set; }
    public IEnumerable<string> ValidationErrors { get; protected set; }

    public GlobalResponse() { }

    [System.Text.Json.Serialization.JsonConstructor]
    private GlobalResponse(bool succeded, Error error, IReadOnlyList<string> validationErrors)
    {
        Succeded = succeded;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static GlobalResponse Success()
    {
        var result = new GlobalResponse()
        {
            Succeded = true
        };

        return result;
    }

    public static async Task<GlobalResponse> SuccessAsync() => await Task.FromResult(Success());

    public static GlobalResponse Fail()
    {
        var result = new GlobalResponse()
        {
            Succeded = false
        };

        return result;
    }

    public static GlobalResponse Fail(string message)
    {
        var result = new GlobalResponse()
        {
            Succeded = false,
            Error = new Error() { Message = message }
        };

        return result;
    }

    public static GlobalResponse Fail(int code, string message)
    {
        var result = new GlobalResponse()
        {
            Succeded = false,
            Error = new Error()
            {
                Code = code,
                Message = message
            }
        };

        return result;
    }

    public static GlobalResponse Fail(int code, string message, IReadOnlyList<string> validations)
    {
        var result = new GlobalResponse()
        {
            Succeded = false,
            ValidationErrors = validations,
            Error = new Error()
            {
                Code = code,
                Message = message,
            }
        };

        return result;
    }

    public static async Task<GlobalResponse> FailAsync() => await Task.FromResult(Fail());
    public static async Task<GlobalResponse> FailAsync(string message) => await Task.FromResult(Fail(message));
    public static async Task<GlobalResponse> FailAsync(int code, string message) => await Task.FromResult(Fail(code, message));
    public static async Task<GlobalResponse> FailAsync(int code, string message, IReadOnlyList<string> validations) => await Task.FromResult(Fail(code, message, validations));

    public static GlobalResponse Validations(IEnumerable<string> validations)
    {
        var result = new GlobalResponse()
        {
            Succeded = false,
            ValidationErrors = validations
        };

        return result;
    }

    public static async Task<GlobalResponse> ValidationsAsync(IEnumerable<string> validations) => await Task.FromResult(Validations(validations));
}

public class GlobalResponse<T> : GlobalResponse, IGlobalResponse<T>
{
    public Payload<T> Payload { get; protected set; }

    public GlobalResponse() : base() { }

    [System.Text.Json.Serialization.JsonConstructor]
    private GlobalResponse(bool succeded, Error error, IReadOnlyList<string> validationErrors, Payload<T> payload)
    {
        Succeded = succeded;
        Error = error;
        ValidationErrors = validationErrors;
        Payload = payload;
    }

    public static new GlobalResponse<T> Success()
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = true
        };

        return result;
    }

    public static GlobalResponse<T> Success(T data)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = true,
            Payload = new Payload<T>() { Data = data }
        };

        return result;
    }
    public static GlobalResponse<T> Success(T data, int page, int pageSize, int total)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = true,
            Payload = new Payload<T>() { Data = data, Page = page, PageSize = pageSize, Total = total }
        };

        return result;
    }
    public static new async Task<GlobalResponse<T>> SuccessAsync() => await Task.FromResult(Success());
    public static async Task<GlobalResponse<T>> SuccessAsync(T data) => await Task.FromResult(Success(data));
    public static async Task<GlobalResponse<T>> SuccessAsync(T data, int page, int pageSize, int total) => await Task.FromResult(Success(data, page, pageSize, total));

    public static new GlobalResponse<T> Fail()
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false
        };

        return result;
    }

    public static new GlobalResponse<T> Fail(string message)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false,
            Error = new Error()
            {
                Message = message
            }
        };

        return result;
    }

    public static new GlobalResponse<T> Fail(int code, string message)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false,
            Error = new Error()
            {
                Code = code,
                Message = message
            }
        };

        return result;
    }

    public static new GlobalResponse<T> Fail(int code, string message, IReadOnlyList<string> validations)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false,
            ValidationErrors = validations,
            Error = new Error()
            {
                Code = code,
                Message = message,
            }
        };

        return result;
    }

    public static GlobalResponse<T> Fail(T data)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false,
            Payload = new Payload<T>() { Data = data }
        };

        return result;
    }

    public static new async Task<GlobalResponse<T>> FailAsync() => await Task.FromResult(Fail());
    public static new async Task<GlobalResponse<T>> FailAsync(string message) => await Task.FromResult(Fail(message));
    public static new async Task<GlobalResponse<T>> FailAsync(int code, string message) => await Task.FromResult(Fail(code, message));
    public static new async Task<GlobalResponse<T>> FailAsync(int code, string message, IReadOnlyList<string> validations) => await Task.FromResult(Fail(code, message, validations));

    public static async Task<GlobalResponse<T>> FailAsync(T data) => await Task.FromResult(Fail(data));

    public static new GlobalResponse<T> Validations(IEnumerable<string> validations)
    {
        var result = new GlobalResponse<T>()
        {
            Succeded = false,
            ValidationErrors = validations
        };

        return result;
    }

    public static new async Task<GlobalResponse<T>> ValidationsAsync(List<string> validations) => await Task.FromResult(Validations(validations));
}

public class Error : IError
{
    public int Code { get; set; }
    public string Message { get; set; }
}

public class Payload<T> : IPayload<T>
{
    public T Data { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public int? Total { get; set; }
}

public class NullClass : INullClass
{
}
