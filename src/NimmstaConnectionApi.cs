using Nimmsta.Net.CoreApi;
using Nimmsta.Net.CoreApi.Request;
using Nimmsta.Net.CoreApi.Response;
using System.Collections.Concurrent;
using System.Text.Json.Nodes;

namespace Nimmsta.Net;

/// <summary>
/// All requests that are concerned with establishing connection,
/// cancelling a connection attempt or disconnecting from a device.
/// General Requests do not need a connected device in order to be executed.
/// <br/> NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/3connectionRequests/"/>
/// </summary>
public sealed class NimmstaConnectionApi : IDisposable
{
    private readonly NimmstaWebSocketClient nimmstaApi;

    public NimmstaConnectionApi(NimmstaWebSocketClient nimmstaApiClient)
    {
        nimmstaApi = nimmstaApiClient;
        nimmstaApi.RegisterEventHandler(CheckReceivedDeviceActionEvent);
    }

    public void Dispose()
    {
        nimmstaApi.UnregisterEventHandler(CheckReceivedDeviceActionEvent);
        while (!Devices.IsEmpty)
        {
            var nextKey = Devices.FirstOrDefault().Key;
            if (Devices.TryRemove(nextKey, out var deviceApi))
                deviceApi.Dispose();
        }
    }

    public Action<NimmstaDeviceApi, NimmstaEvent?>? DeviceConnected { get; set; }
    public Action<NimmstaDeviceApi?, NimmstaEvent?>? DeviceDisconnected { get; set; }

    public ConcurrentDictionary<string, NimmstaDeviceApi> Devices { get; } = [];
    public ConcurrentDictionary<string, JsonNode?> NearbyDevices { get; } = [];

    private bool CheckReceivedDeviceActionEvent(NimmstaEvent nimmstaEvent)
    {
        switch (nimmstaEvent.EventName)
        {
            case "ConnectEvent":
                if (!Devices.ContainsKey(nimmstaEvent.DeviceAddress))
                {
                    var newConnectedDevice = new NimmstaDeviceApi(nimmstaApi, nimmstaEvent.DeviceAddress);
                    if (Devices.TryAdd(nimmstaEvent.DeviceAddress, newConnectedDevice))
                    {
                        DeviceConnected?.Invoke(newConnectedDevice, nimmstaEvent);
                        newConnectedDevice.CheckReceivedDeviceActionEvent(nimmstaEvent);
                    }

                    return true;
                }
                return false;

            case "DisconnectEvent":
                if (Devices.Remove(nimmstaEvent.DeviceAddress, out var nimmstaDevice))
                {
                    nimmstaDevice.CheckReceivedDeviceActionEvent(nimmstaEvent);
                    DeviceDisconnected?.Invoke(nimmstaDevice, nimmstaEvent);
                    return true;
                }

                DeviceDisconnected?.Invoke(null, nimmstaEvent);
                return false;

            case "DeviceFound":
                NearbyDevices.TryAdd(nimmstaEvent.DeviceAddress, nimmstaEvent.EventData);
                CancelSearchAsync().Wait();
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Displays the connection screen (on the host running NIMMSTA application).
    /// </summary>
    public async Task<bool> DisplayConnectActivityAsync()
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("DisplayConnectActivity"));

        var response = await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);

        return response.WasSuccessfull;
    }

    public async Task<bool> SeachForConnectedDevicesAsync()
    {
        var nimmstaDevices = await GetDevicesAsync();
        if (nimmstaDevices != null && nimmstaDevices.Count > 0)
        {
            foreach (var nimmstaDeviceInfo in nimmstaDevices)
            {
                var virtualNimmstaEvent = new NimmstaEvent(new JsonObject()
                {
                    { "event", new JsonObject() { { "name", "ConnectEvent" } } },
                    { "device", (string)nimmstaDeviceInfo!["address"]! }
                });

                if (!Devices.ContainsKey(virtualNimmstaEvent.DeviceAddress))
                {
                    var newConnectedDevice = new NimmstaDeviceApi(nimmstaApi, virtualNimmstaEvent.DeviceAddress);
                    if (Devices.TryAdd(virtualNimmstaEvent.DeviceAddress, newConnectedDevice))
                    {
                        DeviceConnected?.Invoke(newConnectedDevice, virtualNimmstaEvent);
                        newConnectedDevice.CheckReceivedDeviceActionEvent(virtualNimmstaEvent);
                    }
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Starts searching for connectable devices. If a device is found a DeviceFoundEvent is sent.
    /// </summary>
    /// <param name="checkPermissions">
    /// optional, default: false. Will check permissions using an activity before scanning.
    /// </param>
    public async Task SearchForDevicesAsync(bool? checkPermissions = null)
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("SearchForNimmstaDevices"));

        if (checkPermissions != null)
            requestMessage.MessageData = new JsonObject { { "checkPermissions", checkPermissions.Value } };

        await nimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// Cancels currently active search for devices action.
    /// </summary>
    public async Task CancelSearchAsync()
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("CancelSearch"));

        await nimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// Enable / Disable Multi Device
    /// </summary>
    /// <param name="allowMultiDevice">
    /// Enables or disables the ability to connect multiple devices.
    /// </param>
    public async Task SetConnectionManagerSettingsAsync(bool allowMultiDevice)
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("SetConnectionManagerSettings")
            {
                ActionData = new JsonObject { { "allowMultiDevice", allowMultiDevice } }
            });

        await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);
    }

    /// <summary>
    /// Gets a connection code that can be displayed, so the user can scan it and connect.
    /// <br/>Returns via action response message:
    /// <br/>- connectionCode(string) Connection code to display, including the prefix.
    /// <br/>- qrCodeImage(string) Base64-encoded barcode image(PNG), size 300px x 300px.
    /// </summary>
    public async Task<JsonNode?> GetConnectionCodeAsync()
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("GetConnectionCode"));

        var responseMessage = await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);
        return responseMessage.ActionData;
    }

    /// <summary>
    /// Connect to a device using a connection code.
    /// The connection code consists of a 5-digit code and the prefix
    /// <code>XXConnectHSD50XX:</code> or a 6-digit code and the prefix
    /// <code>XXConnectHST50XX:</code> for temporary connection codes,
    /// which the device forgets on shutdown.
    /// </summary>
    /// <param name="connectionCode">e.g. 'XXConnectHSD50XX:5F9OV'</param>
    /// <param name="checkPermissions">
    /// optional, default: false. Will check permissions using an activity before scanning.
    /// </param>
    public async Task ConnectToConnectionCodeAsync(string connectionCode,
        bool? checkPermissions = null)
    {
        var actionData = new JsonObject { { "connectionCode", connectionCode } };
        if (checkPermissions != null)
            actionData.Add("checkPermissions", checkPermissions.Value);

        var requestAction = new NimmstaRequestAction("ConnectToConnectionCode")
        {
            ActionData = actionData
        };

        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            requestAction);

        await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);
    }

    /// <summary>
    /// Connect to a device using an address.
    /// </summary>
    /// <param name="deviceAddress"></param>
    /// <param name="checkPermissions">
    /// optional, default: false. Will check permissions using an activity before scanning.
    /// </param>
    public async Task ConnectToAddressAsync(string deviceAddress,
        bool? checkPermissions = null)
    {
        var actionData = new JsonObject { { "address", deviceAddress } };
        if (checkPermissions != null)
            actionData.Add("checkPermissions", checkPermissions.Value);

        var requestAction = new NimmstaRequestAction("ConnectToAddress")
        {
            ActionData = actionData
        };

        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            requestAction);

        await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);
    }

    /// <summary>
    /// In order to check if all permissions are set and bluetooth is enabled.
    /// <br>Returns empty response if all was good, error otherwise.</br>
    /// <br>Requires NIMMSTA App Version 7.0.5+</br>
    /// </summary>
    /// <returns></returns>
    public async Task CheckForPermissionsUsingActivityAsync()
    {
        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            new NimmstaRequestAction("CheckForPermissionsUsingActivity"));

        await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);
    }

    /// <summary>
    /// Returns a list of all devices by a given filter. Filter is one of
    /// - ALL - CONNECTED (default) - CONNECTING - CHARGING - SEARCHING_TO_RECONNECT
    /// <br/>Returns via action response message:
    /// <br/>'devices' (array of strings) List of devices found.
    /// </summary>
    /// <param name="filter">
    /// One of
    /// <br/>- ALL
    /// <br/>- CONNECTED
    /// <br/>- CONNECTING
    /// <br/>- CHARGING
    /// <br/>- SEARCHING_TO_RECONNECT
    /// </param>
    public async Task<JsonArray?> GetDevicesAsync(string filter = "CONNECTED")
    {
        var actionData = new JsonObject { { "filter", filter } };
        var requestAction = new NimmstaRequestAction("GetDevices")
        {
            ActionData = actionData
        };

        var requestMessage = new NimmstaGeneralRequest(
            nimmstaApi.GetNextRequestId(),
            requestAction);

        var responseMessage = await nimmstaApi.SendAndWaitForResponseAsync(requestMessage);

        return responseMessage.ActionData?["devices"]?.AsArray();
    }
}
