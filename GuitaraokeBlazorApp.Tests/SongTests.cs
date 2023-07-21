using GuitaraokeBlazorApp.Endpoints;

namespace GuitaraokeBlazorApp.Tests;

public class SongTests {

	[Fact]
	public async Task Starring_Song_Toggles_Star_On() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var user = new User();
		var tracker = new FakeUserTracker(user);
		await SongEndpoints.ToggleSongStar(hurt.Slug, db, tracker);
		db.ListStarredSongs(user).Count().ShouldBe(1);
		await SongEndpoints.ToggleSongStar(hurt.Slug, db, tracker);
		db.ListStarredSongs(user).Count().ShouldBe(0);
	}

	[Fact]
	public async Task POST_Song_Signs_Up_For_Song() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", new[] { Instrument.Sing, Instrument.BassGuitar }, db, tracker);
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

		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar }, db, tracker);
		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", Array.Empty<Instrument>(), db, tracker);

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
		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar }, db, tracker);
		await SongEndpoints.UpdateSong(hurt.Slug, "User 2", new[] { Instrument.BassGuitar }, db, new FakeUserTracker(user2));

		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", new Instrument[] { }, db, tracker);

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
		await SongEndpoints.UpdateSong(hurt.Slug, "Surly Dev", instruments, db, tracker);
		db.FindUser(user.Guid)!.Name.ShouldBe("Surly Dev");
	}

}
