namespace Shared.GlobalResponse;

public interface IGlobalResponse
{
    bool Succeded { get; }
    Error Error { get; }
    IEnumerable<string> ValidationErrors { get; }
}

public interface IGlobalResponse<T> : IGlobalResponse
{
    Payload<T> Payload { get; }
}

public interface IError
{
    int Code { get; }
    string Message { get; }
}

public interface IPayload<T>
{
    T Data { get; }
    int? Page { get; }
    int? PageSize { get; }
    int? Total { get; }
}

public interface INullClass
{
}
