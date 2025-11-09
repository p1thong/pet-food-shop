using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Services;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly FCMService _fcmService;
        private readonly IUserService _userService;
        private readonly IFcmTokenService _fcmTokenService;
        private readonly IMessageService _messageService;

        public NotificationController(FCMService fcmService, IUserService userService, IFcmTokenService fcmTokenService, IMessageService messageService)
        {
            _fcmService = fcmService;
            _userService = userService;
            _fcmTokenService = fcmTokenService;
            _messageService = messageService;
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

        [HttpPost("send-all")]
        public async Task<IActionResult> SendNotificationToAllUser([FromBody] NotificationRequest request)
        {
            //var result = await _fcmService.SendToDeviceAsync();

            //return Ok(new { messageId = result });
            return Ok();
        }

        [HttpPost("update-fcm")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] FcmTokenRequest req)
        {
            try
            {
                var user = await _userService.GetUserByIdEntityAsync(req.UserId);
                if (user == null) return NotFound();

                await _fcmTokenService.AddFcmTokenAsync(req.UserId, req.Token);

                // Subscribe to topic based on user role
                await _fcmService.SubscribeToTopicAsync(req.Token, "allUsers");

                return Ok(new { Message = "FCM token updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var message = new Message
            {
                Conversationid = request.ConversationId,
                Orderid = request.OrderId,
                Senderid = request.SenderId,
                Receiverid = request.ReceiverId,
                Message1 = request.Message,
                Createdat = DateTime.UtcNow,
                Isread = false
            };

            await _messageService.SaveNewMessage(message);

            return Ok(new
            {
                success = true,
                data = message
            });
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

    public class FcmTokenRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
