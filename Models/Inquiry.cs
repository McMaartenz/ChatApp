using System.ComponentModel.DataAnnotations;

// Uncomment the following line if EF is expected to not contain nullables
#nullable disable

namespace ChatApp.Models
{
	public class Inquiry
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Onderwerp ontbreekt")]
		[StringLength(200, ErrorMessage = "Onderwerp te lang")]
		public string Topic { get; set; }

		[Required(ErrorMessage = "Email ontbreekt")]
		[EmailAddress(ErrorMessage = "Email is fout")]
		[StringLength(256, ErrorMessage = "Email te lang")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Bericht ontbreekt")]
		[StringLength(600, ErrorMessage = "Bericht te lang")]
		public string Message { get; set; }

		[Required(ErrorMessage = "Captcha ontbreekt")]
		public string Captcha { get; set; }
	}
}

#nullable enable