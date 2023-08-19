using System.Net.WebSockets;
using GuitaraokeWebApp.Hubs;
using GuitaraokeWebApp.Model;
using Microsoft.AspNetCore.SignalR;

namespace GuitaraokeWebApp.Tests;

public abstract class RootControllerTestBase {
	protected List<Song> songs;
	protected ISongDatabase db;
	protected readonly IHubContext<SongHub> hc = new FakeHubContext();

	protected RootController MakeController()
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker(), hc);

	protected RootController MakeController(User user)
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker(user), hc);

	protected RootControllerTestBase() {
		songs = new() { new("Abba", "Waterloo") };
		db = new SongDatabase(songs);
	}

}
