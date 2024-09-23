using Nimmsta.Net.CoreApi.Request;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Nimmsta.Net;

public class NimmstaDeviceUiApi(NimmstaDeviceApi deviceApi)
{
    /// <summary>
    /// To change the contents of the NIMMSTA Smart Watch screen,
    /// you can send a setLayout Request with your own custom XML
    /// that represents a layout the NIMMSTA Smart Watch can understand.
    /// <br/>To learn more about XML layouts have a look at the
    /// <see href="https://docs.nimmsta.com/layout">NIMMSTA Layout Documentation</see>.
    /// <br> If a display timeout is provided, this layout will be displayed
    /// only the provided amount of milliseconds.</br>
    /// </summary>
    /// <param name="nimmstaLayout">
    /// <see href="https://docs.nimmsta.com/layout">NIMMSTA XML Layout</see>
    /// </param>
    /// <param name="layoutData">
    /// Object with key value pairs of dynamic data, that can be updated later
    /// </param>
    /// <param name="displayTimeoutMs">
    /// Timeout in ms (how long should the layout be displayed before switching back)</param>
    /// <returns></returns>
    public async Task SetLayoutAsync(XDocument nimmstaLayout,
        JsonNode? layoutData = null, long? displayTimeoutMs = null)
    {
        var actionData = new JsonObject();

        if (displayTimeoutMs != null)
            actionData.Add("timeout", displayTimeoutMs);

        nimmstaLayout.Declaration ??= new XDeclaration("1.0", "utf-8", null);
        var nimmstaLayoutXmlString = nimmstaLayout.ToString(SaveOptions.DisableFormatting);
        Console.WriteLine("NIMMSTA Layout XML:");
        Console.WriteLine(nimmstaLayoutXmlString);
        actionData.Add("layout", nimmstaLayoutXmlString);

        if (layoutData != null)
            actionData.Add("layoutData", layoutData);

        var requestMessage = new NimmstaDeviceRequest(
            deviceApi.NimmstaApi.GetNextRequestId(),
            deviceApi.DeviceAddress,
            new NimmstaRequestAction(displayTimeoutMs == null ? "SetLayout" : "SetLayoutFor")
            {
                ActionData = actionData
            });

        await deviceApi.NimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// To update an already displayed Layout without
    /// sending the whole layout again, you can call updateLayout.
    /// Update layout requires an object with key value pairs.
    /// The key should be the same as the name of the element
    /// you want to update and the value represents the new value
    /// that should be displayed.
    /// <br/>To learn more about XML layouts have a look at the
    /// <see href="https://docs.nimmsta.com/layout">NIMMSTA Layout Documentation</see>.
    /// </summary>
    public async Task UpdateLayoutAsync(JsonNode layoutData)
    {
        var requestMessage = new NimmstaDeviceRequest(
            deviceApi.NimmstaApi.GetNextRequestId(),
            deviceApi.DeviceAddress,
            new NimmstaRequestAction("UpdateLayout")
            {
                ActionData = new JsonObject
                {
                    { "layoutData", layoutData }
                }
            });

        await deviceApi.NimmstaApi.SendRequestAsync(requestMessage);
    }

}
