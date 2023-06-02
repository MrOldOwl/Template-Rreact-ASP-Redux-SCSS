using Microsoft.AspNetCore.Mvc;
using TestASPReactProject.Controllers.Extentions;

namespace TestASPReactProject.Controllers;

//[ApiController]
//[Route("[controller]")]
public class DetraWebSocketController : ControllerBase {

    [Route("/socket")]
    public async Task Get() {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await webSocket.Echo();
        } else {
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}
