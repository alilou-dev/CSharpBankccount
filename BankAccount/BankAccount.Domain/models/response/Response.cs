using BankAccount.Domain.models;

public class Response
{
    public bool Success { get => Errors.Count == 0; }
    public object Data { get; private set; } = null;
    public List<ResponseError> Errors { get; private set; } = new List<ResponseError>();

    public Response()
    {

    }

    public Response(object data)
    {
        Data = data;
    }

    public Response(List<ResponseError> errors)
    {
        Errors = errors;
    }

    public Response(object data, List<ResponseError> errors)
    {
        Data = data;
        Errors = errors;
    }
}

public class ResponseError
{
    public ResponseCode ResponseCode { get; private set; }
    public string ResponseLabel { get; private set; }
    public string MessageError { get; private set; } = null;

    public ResponseError(ResponseCode responseCode, string message = null)
    {
        ResponseCode = responseCode;
        ResponseLabel = responseCode.ToString();
        MessageError = message;
    }
}