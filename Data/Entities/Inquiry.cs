using System.ComponentModel.DataAnnotations;

#nullable disable
namespace ChatApp.Data.Entities
{
	public class Inquiry
	{
		[Key]
		public int Id { get; set; }
		public string Topic { get; set; }
		public string Email { get; set; }
		public string Message { get; set; }
		public DateTime Time { get; set; }
	}
}

#nullable enable
