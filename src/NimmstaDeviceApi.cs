using Nimmsta.Net.CoreApi;
using Nimmsta.Net.CoreApi.Request;
using Nimmsta.Net.CoreApi.Response;
using System.Text.Json.Nodes;

namespace Nimmsta.Net;

/// <summary>
/// Device Requests need a connected device in order to be executed.
/// If no device is provided, the first connected device is used.
/// NIMMSTA docs: <see href="https://docs.nimmsta.com/websocket/7.0/3connectionRequests/#device-requests"/>
/// </summary>
public sealed class NimmstaDeviceApi : IDisposable
{
    public NimmstaDeviceApi(NimmstaWebSocketClient nimmstaApiClient,
        string deviceAddress)
    {
        NimmstaApi = nimmstaApiClient;
        DeviceAddress = deviceAddress;

        Events = new NimmstaDeviceEvents(this);
        Ui = new NimmstaDeviceUiApi(this);

        NimmstaApi.RegisterEventHandler(CheckReceivedDeviceActionEvent);
    }

    public void Dispose()
    {
        NimmstaApi.UnregisterEventHandler(CheckReceivedDeviceActionEvent);
    }

    public NimmstaWebSocketClient NimmstaApi { get; }
    public string DeviceAddress { get; }
    public NimmstaDeviceEvents Events { get; }
    public NimmstaDeviceUiApi Ui { get; }

    internal bool CheckReceivedDeviceActionEvent(NimmstaEvent nimmstaEvent)
    {
        if (nimmstaEvent.DeviceAddress != DeviceAddress)
            return false;

        switch (nimmstaEvent.EventName)
        {
            case "ConnectEvent":
                DeviceConnected?.Invoke(nimmstaEvent);
                return true;
            case "DisconnectEvent":
                DeviceDisconnected?.Invoke(nimmstaEvent);
                return true;
            default:
                return Events.CheckReceivedDeviceActionEvent(nimmstaEvent);
        }
    }

    public override int GetHashCode()
        => DeviceAddress.GetHashCode();

    /// <summary>
    /// Sent when a device was successfully connected.
    /// <br/>
    /// <br>reconnect (boolean) True if this is a reconnect, false otherwise</br>
    /// </summary>
    public Action<NimmstaEvent>? DeviceConnected { get; set; }

    /// <summary>
    /// Disconnect events occur when a NIMMSTA Smart Watch
    /// gets disconnected for whatever reason. The returned event contains
    /// the reason of the disconnect.
    /// <br/>
    /// <br>reason(string) Reason for the disconnect.</br>
    /// </summary>
    public Action<NimmstaEvent>? DeviceDisconnected { get; set; }

    /// <summary>
    /// Checks if a given device is connected.
    /// </summary>
    public async Task CheckIsConnectedAsync()
    {
        var requestMessage = new NimmstaDeviceRequest(
            NimmstaApi.GetNextRequestId(),
            DeviceAddress,
            new NimmstaRequestAction("IsConnected"));

        await NimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// Cancel connection attempt with a device.
    /// You can either supply an address or a ConnectionCode.
    /// The device address is used by default, if no ConnectionCode is provided.
    /// </summary>
    /// <param name="connectionCode">e.g. XXConnectHSD50XX:5F9OV</param>
    public async Task CancelConnectAsync(string? connectionCode = null)
    {
        var requestAction = new NimmstaRequestAction("CancelConnect");

        if (string.IsNullOrEmpty(connectionCode))
            requestAction.ActionData = new JsonObject { { "connectionCode", connectionCode } };
        else
            requestAction.ActionData = new JsonObject { { "address", DeviceAddress } };

        var requestMessage = new NimmstaDeviceRequest(
            NimmstaApi.GetNextRequestId(),
            DeviceAddress,
            requestAction);

        await NimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// Cancels all connection attempts.
    /// </summary>
    public async Task CancelAllConnectAsync()
    {
        var requestMessage = new NimmstaDeviceRequest(
            NimmstaApi.GetNextRequestId(),
            DeviceAddress,
            new NimmstaRequestAction("CancelAllConnect"));

        await NimmstaApi.SendRequestAsync(requestMessage);
    }

    /// <summary>
    /// Disconnects from a given device.
    /// You can set shutdown to true in order to also shutdown.
    /// </summary>
    public async Task DisconnectDeviceAsync(bool shutdownDevice = false)
    {
        var requestMessage = new NimmstaDeviceRequest(
            NimmstaApi.GetNextRequestId(),
            DeviceAddress,
            new NimmstaRequestAction("Disconnect")
            {
                ActionData = new JsonObject { { "shutdown", shutdownDevice } }
            });

        await NimmstaApi.SendRequestAsync(requestMessage);
    }
}
