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

	public Guid GetUserGuid()
		=> Guid.TryParse(cookieValue, out var guid) ? guid : Guid.Empty;
}