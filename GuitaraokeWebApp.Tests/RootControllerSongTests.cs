using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests;

public class RootControllerSongTests : RootControllerTestBase {

	[Fact]
	public async Task Song_Returns_Song() {
		var c = MakeController();
		var song = songs.First();
		var result = await c.Song(song.Slug) as ViewResult;
		result.ShouldNotBeNull();
		var selection = result.Model as SongSelection;
		selection.Song.ShouldBe(song);
	}

	[Fact]
	public async Task Song_Returns_Song_With_Star() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var guid = Guid.NewGuid();
		db.ToggleStar(guid, hurt);
		var tracker = new FakeUserTracker(guid);
		var c = new RootController(new NullLogger<RootController>(), db, tracker);
		var result = await c.Song(hurt.Slug) as ViewResult;
		var model = result.Model as SongSelection;
		model.IsStarred.ShouldBe(true);
	}

}