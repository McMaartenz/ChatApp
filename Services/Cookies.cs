namespace ChatApp.Services
{
    public class Cookies
    {
        private readonly HttpContext _ctx;

        public Cookies(HttpContext ctx)
        {
            _ctx = ctx;
        }

        public string this[string name]
        {
            get => _ctx.Request.Cookies[name] ?? string.Empty;
        }

        public void Set(string name, string value, DateTimeOffset? expires = null)
        {
            _ctx.Response.Cookies.Append(name, value, new CookieOptions
            {
                Expires = expires ?? DateTimeOffset.Now.AddDays(1)
            });
        }
    }
}
