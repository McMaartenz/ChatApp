using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ChatApp.Services;

namespace ChatApp.Middlewares
{
    public class RequestCountMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCountMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            Cookies cookies = new(ctx);

            bool useCookies = cookies["gdpr"] == "accept";
            if (useCookies)
            {
                string requestCount = cookies["requestCount"];
                bool valid = int.TryParse(requestCount, out int count);
                if (!valid)
                {
                    count = 0;
                }

                cookies.Set("requestCount", $"{++count}");
            }

            await _next(ctx);
        }
    }
}
