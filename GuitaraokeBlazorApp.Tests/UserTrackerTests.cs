namespace GuitaraokeBlazorApp.Tests;

public class UserTrackerTests {

	public class Jar : ICookieJar {
		private readonly Dictionary<string, string> contents = new();
		public string? Get(string name) => contents.GetValueOrDefault(name);
		public void Set(string name, string value) => contents[name] = value;
	}

	[Fact]
	public void Tracker_Returns_Same_Instance_For_Same_Guid() {
		var jar = new Jar();
		var guid = Guid.NewGuid();
		var db = new SongDatabase(new Song[] { });
		jar.Set(HttpCookieUserTracker.COOKIE_NAME, guid.ToString());
		var t = new HttpCookieUserTracker(jar, db);
		var user1 = t.GetUser();
		var user2 = t.GetUser();
		user1.Guid.ShouldBe(user2.Guid);
		user1.ShouldBe(user2);
	}

}
