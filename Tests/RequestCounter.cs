using ChatApp.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Tests
{
	public class RequestCounter
	{
		[Theory]
		[InlineData("refuse")]
		[InlineData("")]
		[InlineData((string)null)]
		public async void NotTrackedIfGdprRefused(string gdprString)
		{
			Mock<HttpContext> mockHttpContext = new();
			mockHttpContext.SetupGet(x => x.Request.Cookies["gdpr"]).Returns(gdprString);
			mockHttpContext.SetupGet(x => x.Request.Cookies["requestCount"]).Returns((string)null);

			HttpContext? capturedContext = null;

			Task requestDelegate(HttpContext ctx)
			{
				capturedContext = ctx;
				return Task.CompletedTask;
			}

			Mock<IResponseCookies> mockResponseCookies = new();
			mockHttpContext.SetupGet(x => x.Response.Cookies).Returns(mockResponseCookies.Object);

			RequestCountMiddleware requestCount = new(requestDelegate);
			await requestCount.InvokeAsync(mockHttpContext.Object);

			Assert.NotNull(capturedContext);
			Assert.Empty(mockResponseCookies.Invocations);
		}

		[Fact]
		public async void IfNoneProvidesOne()
		{
			Mock<HttpContext> mockHttpContext = new();
			mockHttpContext.SetupGet(x => x.Request.Cookies["gdpr"]).Returns("accept");
			mockHttpContext.SetupGet(x => x.Request.Cookies["requestCount"]).Returns((string)null);
			
			HttpContext? capturedContext = null;

			Task requestDelegate(HttpContext ctx)
			{
				capturedContext = ctx;
				return Task.CompletedTask;
			}

			Mock<IResponseCookies> mockResponseCookies = new();
			mockHttpContext.SetupGet(x => x.Response.Cookies).Returns(mockResponseCookies.Object);

			RequestCountMiddleware requestCount = new(requestDelegate);
			await requestCount.InvokeAsync(mockHttpContext.Object);

			Assert.NotNull(capturedContext);

			Assert.NotEmpty(mockResponseCookies.Invocations);
			IInvocation appendInvocation = mockResponseCookies.Invocations[0];

			Assert.Equal("Append", appendInvocation.Method.Name);
			Assert.Equal("requestCount", appendInvocation.Arguments[0]);
			Assert.Equal("1", appendInvocation.Arguments[1]);
		}
	}
}