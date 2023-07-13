using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests;

public class RootControllerQueueTests : RootControllerTestBase {

	[Fact]
	public async Task Queue_Returns_Queue() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var lump = new Song("The Presidents of the United States of America", "Lump");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var guid1 = Guid.NewGuid();
		var guid2 = Guid.NewGuid();
		var guid3 = Guid.NewGuid();
		foreach (var guid in new[] { guid1, guid2, guid3 }) db.ToggleStar(guid, hurt);
		foreach (var guid in new[] { guid1, guid2 }) db.ToggleStar(guid, torn);
		foreach (var guid in new[] { guid1 }) db.ToggleStar(guid, once);
		var c = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker());
		var result = await c.Queue() as ViewResult;
		var model = result.Model as SongQueue;
		model.ShouldNotBeNull();
		model.StarredSongs[hurt].ShouldBe(3);
		model.StarredSongs[torn].ShouldBe(2);
		model.StarredSongs[once].ShouldBe(1);
		model.StarredSongs.Keys.ShouldNotContain(lump);
	}
}