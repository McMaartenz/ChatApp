using System.ComponentModel.DataAnnotations;

#nullable disable
namespace ChatApp.Data.Entities
{
	public class Channel
	{
		[Key] public int Id { get; set; }
		public string Topic { get; set; }
		public ICollection<Message> Messages { get; set; }
	}
}
#nullable enable
