namespace GuitaraokeWebApp.Data;

public class HttpCookieUserTracker : IUserTracker {
	private readonly ICookieJar cookies;
	private readonly ISongDatabase db;
	public const string COOKIE_NAME = "guitaraoke-user-guid";

	public HttpCookieUserTracker(ICookieJar cookies, ISongDatabase db) {
		this.cookies = cookies;
		this.db = db;
	}

	public User GetUser() {
		var cookieValue = cookies.Get(COOKIE_NAME);

		if (!Guid.TryParse(cookieValue, out var guid)) guid = Guid.NewGuid();
		var user = db.FindUser(guid);
		if (user == default) {
			user = new(guid);
			cookies.Set(COOKIE_NAME, guid.ToString());
			db.SaveUser(user);
		}
		return user;
	}
}