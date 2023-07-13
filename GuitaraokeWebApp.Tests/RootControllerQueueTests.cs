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
		var user1 = new User();
		var user2 = new User();
		var user3 = new User();
		foreach (var user in new[] { user1, user2, user3 }) db.ToggleStar(user, hurt);
		foreach (var user in new[] { user1, user2 }) db.ToggleStar(user, torn);
		foreach (var user in new[] { user1 }) db.ToggleStar(user, once);
		var c = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker());
		var result = await c.Queue() as ViewResult;
		var model = result!.Model as SongQueue;
		model.ShouldNotBeNull();
		model.StarredSongs[hurt].ShouldBe(3);
		model.StarredSongs[torn].ShouldBe(2);
		model.StarredSongs[once].ShouldBe(1);
		model.StarredSongs.Keys.ShouldNotContain(lump);
	}
}