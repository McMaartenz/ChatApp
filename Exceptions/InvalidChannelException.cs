namespace ChatApp.Exceptions
{
	public class InvalidChannelException : Exception
	{
		private const string InvalidChannelMessage = "Channel does not exist.";

		public InvalidChannelException() : base(InvalidChannelMessage)
		{
		}

		public InvalidChannelException(string message) : base(message)
		{
		}

		public InvalidChannelException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
