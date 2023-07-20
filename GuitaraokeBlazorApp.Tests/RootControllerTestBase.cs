namespace GuitaraokeBlazorApp.Tests;

public abstract class RootControllerTestBase {
	protected List<Song> songs;
	protected ISongDatabase db;

	protected RootController MakeController()
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker());

	protected RootController MakeController(User user)
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker(user));

	protected RootControllerTestBase() {
		songs = new() { new("Abba", "Waterloo") };
		db = new SongDatabase(songs);
	}

}
