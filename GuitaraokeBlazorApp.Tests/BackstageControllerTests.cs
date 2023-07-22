using GuitaraokeBlazorApp.Endpoints;

namespace GuitaraokeBlazorApp.Tests;

public class BackstageControllerTests {

	[Fact]
	public async Task BackstagePlayedReturnsOk() {
		var song = new Song("Battle Beast", "Familiar Hell");
		var songs = new List<Song>() { song };
		var db = new SongDatabase(songs);
		var result = await BackstageEndpoints.Status(song.Slug, true, db);
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
		await SongEndpoints.UpdateSong(song.Slug, "Benny", new[] { Instrument.Sing }, db, new FakeUserTracker(user));
		db.GetQueuedSongs().Single().Song.ShouldBe(song);
		await BackstageEndpoints.Status(song.Slug, true, db);
		db.GetQueuedSongs().ShouldBeEmpty();
	}

	private async Task<SongDatabase> SetUpQueue(User user, params Song[] songs) {
		var db = new SongDatabase(songs);
		var tracker = new FakeUserTracker(user);
		foreach (var song in songs) {
			await SongEndpoints.UpdateSong(song.Slug, "", new[] { Instrument.Sing }, db, tracker);
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
		var result = await BackstageEndpoints.Move(new() {
			Song = cave.Slug,
			Position = 0,
		}, db);
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
		var result = await BackstageEndpoints.Move(new() {
			Song = adia.Slug,
			Position = 1,
		}, db);
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
		await BackstageEndpoints.Move(new() { Song = cave.Slug, Position = Int32.MinValue, }, db);
		var queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(cave.Name);
		queue[1].Song.Name.ShouldBe(adia.Name);
		queue[2].Song.Name.ShouldBe(bang.Name);

		await BackstageEndpoints.Move(new() { Song = cave.Slug, Position = Int32.MaxValue, }, db);
		queue = db.GetQueuedSongs();
		queue[0].Song.Name.ShouldBe(adia.Name);
		queue[1].Song.Name.ShouldBe(bang.Name);
		queue[2].Song.Name.ShouldBe(cave.Name);
	}
}
