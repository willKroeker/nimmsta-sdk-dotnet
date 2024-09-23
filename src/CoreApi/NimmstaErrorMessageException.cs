using Nimmsta.Net.CoreApi.Response;

namespace Nimmsta.Net.CoreApi;

public class NimmstaErrorMessageException : NimmstaException
{
    public NimmstaErrorMessageException(NimmstaActionResponse responseMessage)
        : base(responseMessage.ErrorMessage)
    {
        ResponseMessage = responseMessage;
    }

    public NimmstaResponseMessage ResponseMessage { get; }
}
