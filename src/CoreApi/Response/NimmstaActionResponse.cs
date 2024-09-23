using Nimmsta.Net.CoreApi.Request;
using System.Text.Json.Nodes;

namespace Nimmsta.Net.CoreApi.Response;

/// <summary>
/// Actions are sent as a response to a request.
/// NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/2overview/#action"/>
/// </summary>
public class NimmstaActionResponse(JsonNode jsonObject)
    : NimmstaResponseMessage(jsonObject, ResponseMessageType.Action)
{
    /// <summary>
    /// Id of the (requested) action. This is the same id that sent with the request.
    /// </summary>
    public int RequestId => (int)ResponseJsonObject["id"]!;

    /// <summary>
    /// Device address of the device on which the action was executed.
    /// </summary>
    public string? DeviceAddress => (string?)ResponseJsonObject["device"];

    /// <summary>
    /// Name of the executed action.
    /// </summary>
    public string ActionName => (string)ResponseJsonObject["action"]!["name"]!;

    /// <summary>
    /// True if the action was successfully executed, false otherwise.
    /// </summary>
    public bool WasSuccessfull => (bool)ResponseJsonObject["action"]!["success"]!;

    /// <summary>
    /// Data associated with the response.
    /// </summary>
    public JsonNode? ActionData => ResponseJsonObject["action"]!["data"];

    /// <summary>
    /// Error message. Null if the Request was successful.
    /// </summary>
    public string? ErrorMessage => (string?)ResponseJsonObject["action"]!["message"];

    /// <summary>
    /// Stack trace of the error. Null if the Request was successful.
    /// </summary>
    public string? ErrorStackTrace => (string?)ResponseJsonObject["action"]!["throwable"];


    /// <summary>
    /// Instance of origin request message, this action response is related to.
    /// </summary>
    public NimmstaRequestMessage? RequestMessage { get; set; }
}
