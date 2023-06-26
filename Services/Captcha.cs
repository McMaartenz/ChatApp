using Newtonsoft.Json.Linq;

namespace ChatApp.Services
{
	public sealed class Captcha
	{
		private static readonly HttpClient http = new();
		private static string SecretKey => Environment.GetEnvironmentVariable("GOOGLE_CAPTCHA_KEY")!;
		private static string API(string token)
		{
			return $"https://www.google.com/recaptcha/api/siteverify?secret={SecretKey}&response={token}";
		}

		public static bool IsValid(string responseToken)
		{
			var response = http.GetAsync(API(responseToken)).Result;
			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			string JSON = response.Content.ReadAsStringAsync().Result;
			dynamic data = JObject.Parse(JSON);

			return data.success;
		}
	}
}
