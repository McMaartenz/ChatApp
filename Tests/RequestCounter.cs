using ChatApp.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Moq;
using Xunit;

namespace Tests
{
	public class RequestCounter
	{
		private readonly Mock<HttpContext> _mockHttpCtx;
		private readonly Mock<IResponseCookies> _mockResponseCookies;
		private readonly RequestCountMiddleware _middleware;

		// Setup
		public RequestCounter()
		{
			_mockHttpCtx = new();
			_mockResponseCookies = new();
			_middleware = new(RequestDelegate);
		}

		private Task RequestDelegate(HttpContext ctx)
		{
			return Task.CompletedTask;
		}

		[Theory]
		[InlineData("refuse")]
		[InlineData("")]
		[InlineData((string)null)]
		public async void NotTrackedIfGdprRefused(string gdprString)
		{
			// Arrange
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["gdpr"]).Returns(gdprString);
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["requestCount"]).Returns((string)null);
			_mockHttpCtx.SetupGet(x => x.Response.Cookies).Returns(_mockResponseCookies.Object);

			// Act
			await _middleware.InvokeAsync(_mockHttpCtx.Object);

			// Assert
			Assert.Empty(_mockResponseCookies.Invocations);
		}

		[Fact]
		public async void IfNoneProvidesOne()
		{
			// Arrange
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["gdpr"]).Returns("accept");
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["requestCount"]).Returns((string)null);
			_mockHttpCtx.SetupGet(x => x.Response.Cookies).Returns(_mockResponseCookies.Object);

			// Act
			await _middleware.InvokeAsync(_mockHttpCtx.Object);

			// Assert
			Assert.NotEmpty(_mockResponseCookies.Invocations);
			IInvocation appendInvocation = _mockResponseCookies.Invocations[0];

			Assert.Equal("Append", appendInvocation.Method.Name);
			Assert.Equal("requestCount", appendInvocation.Arguments[0]);
			Assert.Equal("1", appendInvocation.Arguments[1]);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		public async void IncreasesRequestCountIfSet(int requestCountData)
		{
			// Arrange
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["gdpr"]).Returns("accept");
			_mockHttpCtx.SetupGet(x => x.Request.Cookies["requestCount"]).Returns(requestCountData.ToString());
			_mockHttpCtx.SetupGet(x => x.Response.Cookies).Returns(_mockResponseCookies.Object);

			// Act
			await _middleware.InvokeAsync(_mockHttpCtx.Object);

			// Assert
			Assert.NotEmpty(_mockResponseCookies.Invocations);
			IInvocation appendInvocation = _mockResponseCookies.Invocations[0];

			Assert.Equal("Append", appendInvocation.Method.Name);
			Assert.Equal("requestCount", appendInvocation.Arguments[0]);
			Assert.Equal($"{requestCountData + 1}", appendInvocation.Arguments[1]);
		}
	}
}
