using System.Net.WebSockets;
using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests;

public abstract class RootControllerTestBase {
	protected readonly List<Song> songs;
	protected readonly ISongDatabase db;

	protected RootController MakeController()
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker());

	protected RootControllerTestBase() {
		songs = new() { new("Abba", "Waterloo") };
		db = new SongDatabase(songs);
	}

}