namespace GuitaraokeWebApp.Data;

public class HttpCookieJar : ICookieJar {
	private readonly IHttpContextAccessor http;

	public HttpCookieJar(IHttpContextAccessor http) {
		this.http = http;
	}

	public string? Get(string name)
		=> http?.HttpContext?.Request.Cookies[name];

	public void Set(string name, string value) {
		var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) };
		http.HttpContext?.Response.Cookies.Append(name, value, options);
	}
}