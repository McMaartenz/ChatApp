using AutoMapper;
using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ChatApp.Controllers
{
	public class EmailController : BaseController
	{
		private const string ContactPageURL = "~/Views/Home/Contact.cshtml";

		private readonly InquiryDbCtx _inquiryDbCtx;
		private readonly IMapper _mapper;

		public EmailController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<EmailController> logger,
			InquiryDbCtx inquiryDbCtx,
			IMapper mapper) : base(userManager, signInManager, logger)
		{
			_inquiryDbCtx = inquiryDbCtx;
			_mapper = mapper;
		}

		public async Task<IActionResult> Send(Inquiry inquiry)
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

				if (!CaptchaService.IsValid(inquiry.Captcha))
				{
					TempData["error:captcha"] = "Captcha ontbreekt";
					return View(ContactPageURL, inquiry);
				}

				// Map to inquiry entity
				Data.Entities.Inquiry dataInquiry = _mapper.Map<Data.Entities.Inquiry>(inquiry);

				// Use DB
				await _inquiryDbCtx.AddAsync(dataInquiry);
				await _inquiryDbCtx.SaveChangesAsync();

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
