using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Entities
{
	public class User
	{
		[Key] public int Id { get; set; }
		public string UserId { get; set; }
		public string UserName { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
