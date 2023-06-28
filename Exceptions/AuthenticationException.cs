namespace ChatApp.Exceptions
{
	public class AuthenticationException : Exception
	{
		private const string NotAuthenticatedMessage = "User is not authenticated.";

		public AuthenticationException() : base(NotAuthenticatedMessage)
		{
		}

		public AuthenticationException(string message) : base(message)
		{
		}

		public AuthenticationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}