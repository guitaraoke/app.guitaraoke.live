namespace GuitaraokeWebApp.Data;

public interface IUserTracker {
	Guid GetUserGuid();
}

public class HttpCookieUserTracker : IUserTracker {
	public const string COOKIE_NAME = "guitaraoke-user-guid";
	private readonly IHttpContextAccessor httpContextAccessor;

	public HttpCookieUserTracker(IHttpContextAccessor httpContextAccessor) {
		this.httpContextAccessor = httpContextAccessor;
	}

	private string? cookieValue
		=> httpContextAccessor.HttpContext?.Request.Cookies[COOKIE_NAME];

	public Guid GetUserGuid() {
		if (Guid.TryParse(cookieValue, out var guid)) return guid;
		guid = Guid.NewGuid();
		var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) };
		httpContextAccessor.HttpContext?.Response.Cookies.Append(COOKIE_NAME, guid.ToString(), options);
		return guid;
	}
}