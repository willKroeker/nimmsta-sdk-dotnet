using Nimmsta.Net.CoreApi.Request;
using Nimmsta.Net.CoreApi.Response;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using Websocket.Client;

namespace Nimmsta.Net.CoreApi;

public class NimmstaWebSocketClient : IDisposable
{
    public static readonly JsonNodeOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly WebsocketClient websocketClient;
    private readonly ConcurrentDictionary<int, NimmstaRequestMessage> pendingRequests = [];
    private readonly ConcurrentDictionary<int, TaskCompletionSource<NimmstaActionResponse>> pendingWaitForResponseTasks = [];

    private readonly List<Func<NimmstaEvent, bool>> registeredEventHandlers = [];

    private volatile int lastRequestId = 0;

    public NimmstaWebSocketClient(string hostname = "localhost",
        int wsPort = 64693, bool useSSl = false)
    {
        var wsProtocoll = useSSl ? "wss" : "ws";
        var url = new Uri($"{wsProtocoll}://{hostname}:{wsPort}");

        websocketClient = new(url)
        {
            ReconnectTimeout = TimeSpan.FromSeconds(30)
        };
        websocketClient.DisconnectionHappened.Subscribe(OnWebSocketDisconnected);
        websocketClient.MessageReceived.Subscribe(OnWebSocketMessageReceived);
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => websocketClient.Dispose();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    public int GetNextRequestId()
        => ++lastRequestId;

    public Action<NimmstaEvent>? WebSocketConnected { get; set; }
    public Action<DisconnectionInfo>? WebSocketDisconnected { get; set; }
    public Action<NimmstaActionResponse>? ActionResponseHandler { get; set; }
    public Action<NimmstaEvent>? EventResponseHandler { get; set; }

    public async Task StartAsync()
        => await websocketClient.Start();
    public async Task StopAsync()
        => await websocketClient.Stop(WebSocketCloseStatus.NormalClosure, "client stopping");

    public void RegisterEventHandler(Func<NimmstaEvent, bool> eventHandler)
        => registeredEventHandlers.Add(eventHandler);

    public void UnregisterEventHandler(Func<NimmstaEvent, bool> eventHandler)
        => registeredEventHandlers.Remove(eventHandler);

    public async Task SendRequestAsync(NimmstaRequestMessage requestMessage)
    {
        pendingRequests.TryAdd(requestMessage.RequestId, requestMessage);
        var requestMessageJson = requestMessage.CreateJsonObject();
        var requestJsonString = requestMessageJson.ToJsonString(
            new System.Text.Json.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            });

#if DEBUG
        Console.WriteLine($"[SND] ws message: {requestJsonString}");
#endif

        await websocketClient.SendInstant(requestJsonString);
    }

    public async Task<NimmstaActionResponse> SendAndWaitForResponseAsync(
        NimmstaRequestMessage requestMessage, bool throwIfNotSuccessful = true)
    {
        var taskCompletionSource = new TaskCompletionSource<NimmstaActionResponse>();
        if (!pendingWaitForResponseTasks.TryAdd(requestMessage.RequestId, taskCompletionSource))
            throw new InvalidOperationException($"Could not add {nameof(TaskCompletionSource)} into pending wait queue for request id '{requestMessage.RequestId}'");

        await SendRequestAsync(requestMessage);
        var responseMssage = await taskCompletionSource.Task;

        pendingWaitForResponseTasks.TryRemove(requestMessage.RequestId, out _);

        if (!responseMssage.WasSuccessfull && throwIfNotSuccessful)
            throw new NimmstaErrorMessageException(responseMssage);

        return responseMssage;
    }

    /// <summary>
    /// Send request to the websocket channel, inserting the message to the queue
    /// and actual sending is done on another thread.
    /// </summary>
    public bool AddRequestToSendQueue(NimmstaRequestMessage requestMessage)
    {
        pendingRequests.TryAdd(requestMessage.RequestId, requestMessage);

        var requestMessageJson = requestMessage.CreateJsonObject();
        var requestJsonString = requestMessageJson.ToJsonString(
            new System.Text.Json.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            });

#if DEBUG
        Console.WriteLine($"[SND-Q] ws message: {requestJsonString}");
#endif

        return websocketClient.Send(requestJsonString);
    }

    private void OnWebSocketDisconnected(DisconnectionInfo disconnectionInfo)
    {
#if DEBUG
        Console.WriteLine($"[RCV] ws disconnected: {disconnectionInfo.CloseStatusDescription}");
#endif
        WebSocketDisconnected?.Invoke(disconnectionInfo);
    }

    private void OnWebSocketMessageReceived(ResponseMessage wsResponseMessage)
    {
#if DEBUG
        Console.WriteLine($"[RCV] ws message: {wsResponseMessage.Text}");
#endif
        var responseMessage = NimmstaResponseMessage.ParseResponse(wsResponseMessage);
        if (responseMessage is NimmstaEvent responseEvent)
        {
            if (responseEvent.EventName == "SocketConnect")
            {
                WebSocketConnected?.Invoke(responseEvent);
                return;
            }

            foreach (var eventHandler in registeredEventHandlers)
            {
                if (eventHandler(responseEvent))
                    return;
            }
            EventResponseHandler?.Invoke(responseEvent);
        }
        else if (responseMessage is NimmstaActionResponse responseAction)
        {
            if (pendingRequests.TryRemove(responseAction.RequestId, out var originRequestMessage))
                responseAction.RequestMessage = originRequestMessage;

            if (pendingWaitForResponseTasks.TryGetValue(responseAction.RequestId, out var taskCompletionSource))
            {
                if (taskCompletionSource.TrySetResult(responseAction))
                    return;
            }

            ActionResponseHandler?.Invoke(responseAction);
        }
        else
        {
#if DEBUG
            Console.WriteLine($"[RCV] unknown type of response message: {responseMessage.ResponseTypeName}");
#endif
        }
    }
}
