namespace GuitaraokeBlazorApp.Tests;

public class BackstageControllerTests {

	[Fact]
	public async Task BackstageIndexReturnsView() {
		var songs = new List<Song>();
		var db = new SongDatabase(songs);
		var bc = new BackstageController(db);
		var result = await bc.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = result.Model as SongQueue;
		model.ShouldNotBeNull();
	}

	[Fact]
	public async Task BackstagePlayedReturnsOk() {
		var song = new Song("Battle Beast", "Familiar Hell");
		var songs = new List<Song>() { song };
		var db = new SongDatabase(songs);
		var bc = new BackstageController(db);
		var result = await bc.Status(song.Slug, true) as JsonResult;
		result.ShouldNotBeNull();
		song = db.FindSong("battle-beast-familiar-hell");
		song.ShouldNotBeNull();
		song.Played.ShouldBe(true);
	}

	[Fact]
	public async Task BackstagePlayed_Removes_Song_From_Queue() {
		var song = new Song("Battle Beast", "Familiar Hell");
		var songs = new List<Song>() { song };
		var db = new SongDatabase(songs);
		var user = new User();
		var rc = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker(user));
		await rc.Song(song.Slug, "Benny", new[] { Instrument.Sing });
		db.GetQueuedSongs().Single().Song.ShouldBe(song);
		var bc = new BackstageController(db);
		await bc.Status(song.Slug, true);
		db.GetQueuedSongs().ShouldBeEmpty();
	}

	private async Task<SongDatabase> SetUpQueue(User user, params Song[] songs) {
		var db = new SongDatabase(songs);
		var rc = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker(user));
		foreach (var song in songs) {
			await rc.Song(song.Slug, "", new[] { Instrument.Sing });
		}
		var queue = db.GetQueuedSongs();
		queue.Select(q => q.Song).ToArray().ShouldBe(songs);
		return db;
	}

	[Fact]
	public async Task Moving_Song_Up_Works() {
		var adia = new Song("Sarah McLachlan", "Adia");
		var bang = new Song("Blur", "Bang");
		var cave = new Song("Muse", "Cave");
		var user = new User();
		var db = await SetUpQueue(user, adia, bang, cave);
		var bc = new BackstageController(db);
		var result = await bc.Move(new() {
			Song = cave.Slug,
			Position = 0,
		});
		result.ShouldNotBeNull();
		var queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(cave.Name);
		queue[1].Song.Name.ShouldBe(adia.Name);
		queue[2].Song.Name.ShouldBe(bang.Name);
	}

	[Fact]
	public async Task Moving_Song_Down_Works() {
		var adia = new Song("Sarah McLachlan", "Adia");
		var bang = new Song("Blur", "Bang");
		var cave = new Song("Muse", "Cave");
		var user = new User();
		var db = await SetUpQueue(user, adia, bang, cave);
		var bc = new BackstageController(db);
		var result = await bc.Move(new() {
			Song = adia.Slug,
			Position = 1,
		});
		result.ShouldNotBeNull();
		var queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(bang.Name);
		queue[1].Song.Name.ShouldBe(adia.Name);
		queue[2].Song.Name.ShouldBe(cave.Name);
	}

	[Fact]
	public async Task Moving_Song_Works_Even_When_Evil_People_Send_Us_Naughty_Json() {
		var adia = new Song("Sarah McLachlan", "Adia");
		var bang = new Song("Blur", "Bang");
		var cave = new Song("Muse", "Cave");
		var user = new User();
		var db = await SetUpQueue(user, adia, bang, cave);
		var bc = new BackstageController(db);
		await bc.Move(new() { Song = cave.Slug, Position = Int32.MinValue, });
		var queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(cave.Name);
		queue[1].Song.Name.ShouldBe(adia.Name);
		queue[2].Song.Name.ShouldBe(bang.Name);

		await bc.Move(new() { Song = cave.Slug, Position = Int32.MaxValue, });
		queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(adia.Name);
		queue[1].Song.Name.ShouldBe(bang.Name);
		queue[2].Song.Name.ShouldBe(cave.Name);
	}
}
