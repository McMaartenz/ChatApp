using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ChatApp.Controllers
{
	public class EmailController : BaseController
	{
		private readonly ILogger<EmailController> _logger;
		private const string ContactPageURL = "~/Views/Home/Contact.cshtml";

		public EmailController(ILogger<EmailController> logger) : base(logger)
		{
			_logger = logger;
		}

		public IActionResult Send(Inquiry inquiry)
		{
			TempData["exception"] = false;
			TempData["success"] = false;
			TempData["error:topic"] = null;
			TempData["error:email"] = null;
			TempData["error:message"] = null;
			TempData["error:captcha"] = null;

			// Perform validation
			try
			{
				ValidationContext ctx = new(inquiry);

				List<ValidationResult> result = new();
				bool isValid = Validator.TryValidateObject(inquiry, ctx, result, true);
				if (!isValid)
				{
					string GetErrorsFor(string member)
					{
						return string.Join("<br>", 
							result.Where(r => r.MemberNames.FirstOrDefault()?.Equals(member) ?? false)
								.Select(r => r.ErrorMessage));
					}

					TempData["error:topic"] = GetErrorsFor("Topic");
					TempData["error:email"] = GetErrorsFor("Email");
					TempData["error:message"] = GetErrorsFor("Message");
					TempData["error:captcha"] = GetErrorsFor("Captcha");
					return View(ContactPageURL, inquiry);
				}

				if (!Captcha.IsValid(inquiry.Captcha))
				{
					TempData["error:captcha"] = "Captcha ontbreekt";
					return View(ContactPageURL, inquiry);
				}

				// Use DB

				// Use API
			}
			catch
			{
				TempData["exception"] = true;
				return View(ContactPageURL, inquiry);
			}

			TempData["success"] = true;
			return View(ContactPageURL, inquiry);
		}
	}
}
