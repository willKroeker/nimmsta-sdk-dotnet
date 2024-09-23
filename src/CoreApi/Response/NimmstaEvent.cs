using System.Text.Json.Nodes;

namespace Nimmsta.Net.CoreApi.Response;

/// <summary>
/// Events are sent by the device, for example a scanner.
/// NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/2overview/#event"/>
/// and <see href="https://docs.nimmsta.com/websocket/7.0/5events/"/>
/// </summary>
public class NimmstaEvent(JsonNode jsonObject)
    : NimmstaResponseMessage(jsonObject, ResponseMessageType.Event)
{
    /// <summary>
    /// Name of the event.
    /// </summary>
    public string EventName => (string)ResponseJsonObject["event"]!["name"]!;

    /// <summary>
    /// Device address where the event originated.
    /// </summary>
    public string DeviceAddress => (string)ResponseJsonObject["device"]!;

    /// <summary>
    /// Event-specific data.
    /// </summary>
    public JsonNode? EventData => ResponseJsonObject["event"]!["data"];
}
