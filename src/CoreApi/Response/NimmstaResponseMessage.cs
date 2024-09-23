using System.Text.Json.Nodes;
using Websocket.Client;

namespace Nimmsta.Net.CoreApi.Response;

/// <summary>
/// NIMMSTA docs <see href="https://docs.nimmsta.com/websocket/7.0/2overview/#responses"/>
/// </summary>
public class NimmstaResponseMessage
{
    /// <summary>
    /// Parsed NIMMSTA response JSON object.
    /// </summary>
    public JsonNode ResponseJsonObject { get; }

    /// <summary>
    /// Type(name) of the response.
    /// </summary>
    public string? ResponseTypeName => (string?)ResponseJsonObject["type"];

    /// <summary>
    /// (Enum)Type of the response.
    /// </summary>
    public ResponseMessageType ResponseType { get; }

    protected NimmstaResponseMessage(JsonNode jsonObject, ResponseMessageType responseType)
    {
        ResponseJsonObject = jsonObject;
        ResponseType = responseType;
    }

    /// <summary>
    /// Tool for quick WebSocket response message parsing to a proper NIMMSTA
    /// response object.
    /// </summary>
    public static NimmstaResponseMessage ParseResponse(ResponseMessage wsResponseMessage)
    {
        if (string.IsNullOrEmpty(wsResponseMessage.Text))
            throw new ArgumentNullException($"{nameof(wsResponseMessage)}.Text", "WebSocket message does not contain 'text' data!");

        var jsonObject = JsonNode.Parse(wsResponseMessage.Text)
            ?? throw new InvalidDataException("WebSocket message is not a valid JSON string!");

        return (string?)jsonObject["type"] switch
        {
            "ACTION" => new NimmstaActionResponse(jsonObject),
            "EVENT" => new NimmstaEvent(jsonObject),
            _ => new NimmstaResponseMessage(jsonObject, ResponseMessageType.Unknown)
        }; ;
    }

    public enum ResponseMessageType
    {
        Unknown,
        Action,
        Event
    }
}
