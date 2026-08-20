using Microsoft.AspNetCore.Http;

public class CommonResponse
{
    public CommonResponse()
    {
    }

    public CommonResponse(
        string message,
        object result,
        bool isSuccess,
        int httpStatusCode)
    {
        Message = message;
        Result = result;
        IsSuccess = isSuccess;
        HttpStatusCode = httpStatusCode;
    }

    public string Message { get; set; }

    public object Result { get; set; }

    public bool IsSuccess { get; set; }

    public int HttpStatusCode { get; set; }

  

    public  CommonResponse Success(object result)
    {
        return new CommonResponse(
            CommonResponseMessage.SuccessMessage,
            result,
            true,
            StatusCodes.Status200OK);
    }

    public static class CommonResponseMessage
    {
        public static string SuccessMessage { get; } = "Request processed successfully.";
    }

    public  CommonResponse Created(object result)
    {
        return new CommonResponse(
            "Created successfully.",
            result,
            true,
            StatusCodes.Status201Created);
    }

    public  CommonResponse BadRequest(string message)
    {
        return new CommonResponse(
            message,
            null,
            false,
            StatusCodes.Status400BadRequest);
    }

    public  CommonResponse NotFound(string message)
    {
        return new CommonResponse(
            message,
            null,
            false,
            StatusCodes.Status404NotFound);
    }

    public CommonResponse NoContent()
    {
        return new CommonResponse(
            "No Content",
            null,
            true,
            StatusCodes.Status204NoContent
            );
    }
}