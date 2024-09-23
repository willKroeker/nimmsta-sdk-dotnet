using System.Text.Json.Nodes;

namespace Nimmsta.Net.CoreApi.Request;

/// <summary>
/// Action to execute.
/// NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/2overview/#requests"/>
/// </summary>
public class NimmstaRequestAction(string name)
{
    /// <summary>
    /// Name of the action. Case-insensitive.
    /// </summary>
    public string ActionName { get; } = name;

    /// <summary>
    /// Additional data specific to the request. Can be undefined or null.
    /// </summary>
    public JsonNode? ActionData { get; set; }

    public virtual JsonNode CreateJsonObject()
    {
        var jsonObject = new JsonObject(NimmstaWebSocketClient.JsonOptions)
        {
            { "name", ActionName }
        };

        if (ActionData != null)
            jsonObject.Add("data", ActionData);

        return jsonObject;
    }
}
