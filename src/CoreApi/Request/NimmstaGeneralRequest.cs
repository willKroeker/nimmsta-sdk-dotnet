namespace Nimmsta.Net.CoreApi.Request;

public class NimmstaGeneralRequest(int id, NimmstaRequestAction action)
    : NimmstaRequestMessage("GENERAL_REQUEST", id, action)
{
}
