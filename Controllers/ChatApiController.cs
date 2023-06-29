using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Data.Entities;
using ChatApp.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ChatApp.Controllers
{
	[ApiController]
	[Route("/api/chatapi/")]
	public class ChatApiController : BaseController
	{
		private readonly ChatService _chatService;

		public ChatApiController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<ChatApiController> logger,
			ChatService chatService)
			: base(userManager, signInManager, logger)
		{
			_chatService = chatService;
		}

		private bool IsAuthenticated()
		{
			if (!Request.Headers.TryGetValue("Authorization", out var headerValue) ||
				!AuthenticationHeaderValue.TryParse(headerValue, out var authHeaderValue) ||
				authHeaderValue.Scheme != "APIKey" ||
				authHeaderValue.Parameter != "ca798148-9038-402a-95c7-c7905e694270")
			{
				return false;
			}

			return true;
		}

		[HttpGet]
		[Route("channels")]
		public async Task<ActionResult<Channel[]>> GetChannels()
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			return Ok(await _chatService.GetChannels());
		}

		[HttpGet]
		[Route("channels/{channelId}")]
		public async Task<ActionResult<Channel[]>> GetChannel(int channelId)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			Channel? channel = await _chatService.GetChannel(channelId);
			if (channel is null)
			{
				return NotFound();
			}
			
			return Ok(channel);
		}

		[HttpGet]
		[Route("channels/{channelId}/messages")]
		public async Task<ActionResult<Message[]>> GetMessages(int channelId)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			Message[] messages = await _chatService.GetMessages(channelId);
			return Ok(messages);
		}

		[HttpPost]
		[Route("messages")]
		public async Task<ActionResult<int>> PostMessage([FromBody] Message message)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			int messageId = await _chatService.PostMessage(message);
			if (messageId == 0)
			{
				return BadRequest();
			}
			
			return Ok(messageId);
		}

		[HttpDelete]
		[Route("messages/{userId}/{messageId}")]
		public async Task<ActionResult> DeleteMessage(int userId, int messageId)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			bool success = await _chatService.DeleteMessage(userId, messageId);
			if (!success)
			{
				return NotFound();
			}

			return Ok();
		}

		[HttpPost]
		[Route("channels")]
		public async Task<ActionResult<int>> PostChannel([FromBody] string topic)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			int channelId = await _chatService.PostChannel(topic);
			if (channelId == 0)
			{
				return BadRequest();
			}

			return Ok(channelId);
		}

		[HttpGet]
		[Route("users/{userId}")]
		public async Task<ActionResult<User>> GetUser(int userId)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			User? user = await _chatService.GetUser(userId);
			if (user is null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		[HttpGet]
		[Route("users/{userId}/id")]
		public async Task<ActionResult<int>> GetUserId(string userId)
		{
			if (!IsAuthenticated())
			{
				return Unauthorized();
			}

			User user = await _chatService.GetUserId(userId);
			return Ok(user.Id);
		}
	}
}
