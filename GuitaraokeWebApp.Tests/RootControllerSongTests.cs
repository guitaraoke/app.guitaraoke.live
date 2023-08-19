using GuitaraokeWebApp.Data;
using GuitaraokeWebApp.Model;
using System.Diagnostics;

namespace GuitaraokeWebApp.Tests;

public class RootControllerSongTests : RootControllerTestBase {

	[Fact]
	public async Task Song_Returns_Song() {
		var c = MakeController();
		var song = songs.First();
		var result = await c.Song(song.Slug) as ViewResult;
		result.ShouldNotBeNull();
		var selection = result.Model as SongSelection;
		selection!.Song.ShouldBe(song);
	}

	[Fact]
	public async Task Song_Returns_Song_With_Star() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.ToggleStar(user, hurt);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		var result = await c.Song(hurt.Slug) as ViewResult;
		var model = result!.Model as SongSelection;
		model!.IsStarred.ShouldBe(true);
	}

	[Fact]
	public async Task POST_Song_Signs_Up_For_Song() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		await c.Song(hurt.Slug, "Surly Dev", new[] { Instrument.Sing, Instrument.BassGuitar });
		var selection = db.FindSongForUser(hurt, user);
		selection.Instruments.ShouldContain(Instrument.Sing);
		selection.Instruments.ShouldContain(Instrument.BassGuitar);
	}

	[Fact]
	public async Task POST_Song_With_No_Instruments_Deletes_Signup() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);

		await c.Song(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar });
		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await c.Song(hurt.Slug, "Surly Dev", new Instrument[] { });

		user.Signups.ShouldBeEmpty();
		queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldNotContain(hurt);
	}


	[Fact]
	public async Task POST_Song_With_No_Instruments_Leaves_Song_In_Queue_If_Other_Signups_Exist() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user1 = new User();
		db.SaveUser(user1);
		var user2 = new User();
		db.SaveUser(user2);
		var tracker = new FakeUserTracker(user1);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		await c.Song(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar });
		await new RootController(new NullLogger<RootController>(), db, new FakeUserTracker(user2), hc).Song(hurt.Slug, "User 2",
			new[] { Instrument.BassGuitar });

		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await c.Song(hurt.Slug, "Surly Dev", new Instrument[] { });

		user1.Signups.ShouldBeEmpty();
		queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
	}


	[Fact]
	public async Task POST_Song_Updates_User_Name_In_Database() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		var instruments = new[] { Instrument.Sing, Instrument.BassGuitar };
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		await c.Song(hurt.Slug, "Surly Dev", instruments);
		db.FindUser(user.Guid)!.Name.ShouldBe("Surly Dev");
	}

	[Fact]
	public async Task POST_Song_Redirects_To_Me() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		var instruments = new[] { Instrument.Sing, Instrument.BassGuitar };
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		var result = await c.Song(hurt.Slug, "Surly Dev", instruments) as RedirectToActionResult;
		result.ShouldNotBeNull();
		result.ActionName.ShouldBe("me");
	}

	[Fact]
	public async Task Song_Returns_Song_With_Name() {
		const string NAME = "Surly Dev";
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User(name: NAME);
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		var result = await c.Song(hurt.Slug) as ViewResult;
		var model = result!.Model as SongSelection;
		model.User.Name.ShouldBe(NAME);
	}


	[Fact]
	public async Task Song_Returns_Song_With_Instruments_Selected() {
		const string NAME = "Surly Dev";
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User(name: NAME);
		user.SignUp(hurt, new[] { Instrument.RhythmGuitar, Instrument.Sing });
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker, hc);
		var result = await c.Song(hurt.Slug) as ViewResult;
		result.ShouldNotBeNull();
		var model = result.Model as SongSelection;
		model.ShouldNotBeNull();
		model.Instruments.Length.ShouldBe(2);
		model.Instruments.ShouldContain(Instrument.RhythmGuitar);
		model.Instruments.ShouldContain(Instrument.Sing);
	}
}
