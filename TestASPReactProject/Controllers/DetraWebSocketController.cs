using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace TestASPReactProject.Controllers;


public class DetraWebSocketController : ControllerBase, IDisposable {

    public List<(WebSocket, TaskCompletionSource<object>)> _webSockets = new();
    private CancellationTokenSource _cts;

    public DetraWebSocketController() {
        _cts = new();
        SendToSocket(_cts.Token);
    }

    [Route("/socket")]
    public async Task Get() {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var token = new TaskCompletionSource<object>();

            _webSockets.Add((webSocket, token));

            // await is needed because HttpContext will be Disposable after the method is completed
            await token.Task;
        } else {
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        }
    }

    private async void SendToSocket(CancellationToken token) {
        while (!token.IsCancellationRequested) {

            foreach (var webSocket in _webSockets.ToArray()) {
                if (webSocket.Item1.CloseStatus.HasValue) {
                    await Delete(webSocket);

                    continue;
                }

                await webSocket.Item1.SendAsync(
                    Encoding.UTF8.GetBytes("Hello!"),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

            }

            await Task.Delay(2_000);
        }

        foreach (var webSocket in _webSockets.ToArray()) {
            await Delete(webSocket);
        }
    }


    private async ValueTask Delete((WebSocket, TaskCompletionSource<object>) webSocket) {
        await webSocket.Item1.CloseAsync(
                                    webSocket.Item1.CloseStatus!.Value,
                                webSocket.Item1.CloseStatusDescription,
                                    CancellationToken.None);

        webSocket.Item2.TrySetResult(webSocket);

        _webSockets.Remove(webSocket);
    }

    public void Dispose() {
        using (_cts) {
            _cts.Cancel();
        }
    }
}
