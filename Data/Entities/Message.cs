using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;

namespace ChatApp.Data.Entities
{
	public class Message
	{
		[Key] public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public bool Deleted { get; set; }

		public int UserId { get; set; }
		public int ChannelId { get; set; }

		public User User { get; set; }
		public Channel Channel { get; set; }
	}
}
