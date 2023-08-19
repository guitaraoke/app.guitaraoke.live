using GuitaraokeWebApp.Hubs;
using GuitaraokeWebApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Serialization;

namespace GuitaraokeWebApp.Tests;

public class RootControllerQueueTests : RootControllerTestBase {

	[Fact]
	public async Task Queue_Returns_Queued_Songs() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var lump = new Song("The Presidents of the United States of America", "Lump");
		db = new SongDatabase(new[] { hurt, torn, once, lump });
		var geddy = new User { Name = "Geddy" };
		var alex = new User { Name = "Alex" };
		var neil = new User { Name = "Neil" };

		db.SaveUser(geddy);
		db.SaveUser(alex);
		db.SaveUser(neil);

		var c = MakeController(geddy);
		await c.Song(hurt.Slug, "Geddy", new[] { Instrument.Sing, Instrument.BassGuitar });
		await c.Song(torn.Slug, "Geddy", new[] { Instrument.RhythmGuitar, Instrument.Sing });
		await c.Song(lump.Slug, "Geddy", new[] { Instrument.BassGuitar });

		c = MakeController(alex);
		await c.Song(hurt.Slug, "Alex", new[] { Instrument.LeadGuitar });
		await c.Song(lump.Slug, "Alex", new[] { Instrument.LeadGuitar });

		c = MakeController(neil);
		await c.Song(hurt.Slug, "Neil", new[] { Instrument.BassGuitar });
		await c.Song(torn.Slug, "Neil", new[] { Instrument.BassGuitar });

		c = MakeController();
		var result = await c.Queue() as ViewResult;
		var model = result!.Model as SongQueue;
		model.ShouldNotBeNull();
		model.QueuedSongs.Count.ShouldBe(3);
		model.QueuedSongs[0].Song.ShouldBe(hurt);
		model.QueuedSongs[0].Players[geddy].ShouldBe(
			new[] { Instrument.BassGuitar, Instrument.Sing }, ignoreOrder: true);
		model.QueuedSongs[0].Players[alex].ShouldBe(new[] { Instrument.LeadGuitar });
		model.QueuedSongs[0].Players[neil].ShouldBe(new[] { Instrument.BassGuitar });

		model.QueuedSongs[1].Song.ShouldBe(torn);
		model.QueuedSongs[2].Song.ShouldBe(lump);
	}

	[Fact]
	public async Task Queue_Returns_Starred_Songs() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var lump = new Song("The Presidents of the United States of America", "Lump");
		var db = new SongDatabase(new[] { hurt, torn, once, lump });
		var user1 = new User();
		var user2 = new User();
		var user3 = new User();
		foreach (var user in new[] { user1, user2, user3 }) db.ToggleStar(user, hurt);
		foreach (var user in new[] { user1, user2 }) db.ToggleStar(user, torn);
		foreach (var user in new[] { user1 }) db.ToggleStar(user, once);
		var c = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker(), hc);
		var result = await c.Queue() as ViewResult;
		var model = result!.Model as SongQueue;
		model.ShouldNotBeNull();
		model.StarredSongs[hurt].ShouldBe(3);
		model.StarredSongs[torn].ShouldBe(2);
		model.StarredSongs[once].ShouldBe(1);
		model.StarredSongs.Keys.ShouldNotContain(lump);
	}
}
