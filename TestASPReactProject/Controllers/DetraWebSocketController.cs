using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace TestASPReactProject.Controllers;


public class DetraWebSocketController : ControllerBase, IDisposable {

    private static Task? _task;

    private static Action<string>? _action;
    private static event Action<string> Event {
        add => _action += value;
        remove => _action -= value;
    }

    public DetraWebSocketController() {
        if (_task == null) {
            _task = Task.Run(async () => {
                int i = 0;
                while (i++ < 100) {
                    _action?.Invoke("OK");

                    await Task.Delay(1000);
                }
            });
        }
    }

    public WebSocket? _webSocket;
    public TaskCompletionSource<object>? _source;

    [Route("/socket")]
    public async Task Get() {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _source = new TaskCompletionSource<object>();

            Event += DetraWebSocketController_Event;

            _webSocket = webSocket;
            // await is needed because HttpContext will be Disposable after the method is completed
            await _source.Task;

            Event -= DetraWebSocketController_Event;
        } else {
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        }
    }

    private async void DetraWebSocketController_Event(string obj) {
        if (_webSocket != null) {
            if (_webSocket.CloseStatus.HasValue)
                Dispose();

            await _webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(obj),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
        }
    }


    private bool _flag_dispose = false;
    public async void Dispose() {
        if (!_flag_dispose) {
            if (_webSocket != null)
                await _webSocket.CloseAsync(
                                           WebSocketCloseStatus.NormalClosure,
                                       _webSocket.CloseStatusDescription,
                                           CancellationToken.None);

            if (_source != null && _webSocket != null)
                _source.TrySetResult(_webSocket);

            _flag_dispose = true;
        }
    }
}
