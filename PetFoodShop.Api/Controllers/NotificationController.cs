using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Services;

namespace PetFoodShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly FCMService _fcmService;

        public NotificationController(FCMService fcmService)
        {
            _fcmService = fcmService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var result = await _fcmService.SendToDeviceAsync(
                token: request.Token,
                title: request.Title,
                body: request.Body,
                type: request.Type,
                data: request.Data
            );

            return Ok(new { messageId = result });
        }
    }

    public class NotificationRequest
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
