using System.Text.Json.Nodes;

namespace Nimmsta.Net.CoreApi.Request;

/// <summary>
/// <list type="bullet">
/// <item>GENERAL_REQUEST Used for getting general information about the API or connection.</item>
/// <item>DEVICE_REQUEST Interaction with the device like setting the LED color or triggering a vibrator burst.</item>
/// <item>LOG Log specific events in the Android or JVM Applications for debugging purposes.</item>
/// </list>
/// NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/2overview/#requests"/>
/// </summary>
public class NimmstaRequestMessage(string requestTypeName, int id, NimmstaRequestAction action)
{
    /// <summary>
    /// Type(name) of the Request.
    /// <list type="bullet">
    /// <item>GENERAL_REQUEST Used for getting general information about the API or connection.</item>
    /// <item>DEVICE_REQUEST Interaction with the device like setting the LED color or triggering a vibrator burst.</item>
    /// <item>LOG Log specific events in the Android or JVM Applications for debugging purposes.</item>
    /// </list>
    /// </summary>
    public string RequestTypeName { get; } = requestTypeName;

    /// <summary>
    /// Id to identify the request. This needs to be set to make sure
    /// your client library is able to match the Response with the correct Request.
    /// Note that you need to implement this logic yourself
    /// since Websockets are asynchronous.
    /// </summary>
    public int RequestId { get; } = id;

    /// <summary>
    /// Device address. Can be undefined, in which case the first connected device will be selected.
    /// </summary>
    public string? DeviceAddress { get; set; }

    /// <summary>
    /// Action to execute.
    /// </summary>
    public NimmstaRequestAction Action { get; } = action;

    /// <summary>
    /// Additional message data specific to the request.
    /// Can be undefined or null.
    /// <br/>Sample usage: <see href="https://docs.nimmsta.com/websocket/7.0/3connectionRequests/#search-for-nimmsta-devices"/>
    /// </summary>
    public JsonNode? MessageData { get; set; }

    public virtual JsonNode CreateJsonObject()
    {
        var jsonObject = new JsonObject()
        {
            { "type", RequestTypeName },
            { "id", RequestId }
        };

        if (!string.IsNullOrEmpty(DeviceAddress))
            jsonObject.Add("device", DeviceAddress);

        jsonObject.Add("action", Action.CreateJsonObject());

        if (MessageData != null)
            jsonObject.Add("data", MessageData);

        return jsonObject;
    }
}
