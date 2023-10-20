using Microsoft.AspNetCore.Http.HttpResults;
using GuitaraokeBlazorApp.Endpoints;

namespace GuitaraokeBlazorApp.Tests.EndpointTests;

public class SongTests {

	[Fact]
	public async Task Starring_Invalid_Slug_Returns_Not_Found() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		var tracker = new FakeUserTracker(user);
		var response = await SongEndpoints.ToggleStar("not-a-real-slug", db, tracker);
		response.ShouldBeOfType<NotFound>();
	}

	[Fact]
	public async Task Starring_Song_Toggles_Star_On() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var user = new User();
		var tracker = new FakeUserTracker(user);
		var response = await SongEndpoints.ToggleStar(hurt.Slug, db, tracker);
		response.ShouldBeOfType<Ok<bool>>();
		((Ok<bool>)response).Value.ShouldBe(true);
		db.ListStarredSongs(user).Count().ShouldBe(1);
		response = await SongEndpoints.ToggleStar(hurt.Slug, db, tracker);
		response.ShouldBeOfType<Ok<bool>>();
		((Ok<bool>) response).Value.ShouldBe(false);
		db.ListStarredSongs(user).Count().ShouldBe(0);
	}

	[Fact]
	public async Task Sign_Up_With_Invalid_Slug_Returns_Not_Found() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var response = await SongEndpoints.SignUpForSong("this-is-a-bad-slug", "Surly Dev", new[] { Instrument.Sing, Instrument.BassGuitar }, db, tracker);
		response.ShouldBeOfType<NotFound>();
	}

	[Fact]
	public async Task Song_Signs_Up_For_Song() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		var response = await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", new[] { Instrument.Sing, Instrument.BassGuitar }, db, tracker);
		response.ShouldBeOfType<Ok>();
		var instrumentsArray = db.FindInstrumentsForUser(hurt, user);
		instrumentsArray.ShouldContain(Instrument.Sing);
		instrumentsArray.ShouldContain(Instrument.BassGuitar);
	}

	[Fact]
	public async Task Song_With_No_Instruments_Deletes_Signup() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);

		await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar }, db, tracker);
		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", Array.Empty<Instrument>(), db, tracker);

		user.Signups.ShouldBeEmpty();
		queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldNotContain(hurt);
	}


	[Fact]
	public async Task Song_With_No_Instruments_Leaves_Song_In_Queue_If_Other_Signups_Exist() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user1 = new User();
		db.SaveUser(user1);
		var user2 = new User();
		db.SaveUser(user2);
		var tracker = new FakeUserTracker(user1);
		await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", new[] { Instrument.LeadGuitar }, db, tracker);
		await SongEndpoints.SignUpForSong(hurt.Slug, "User 2", new[] { Instrument.BassGuitar }, db, new FakeUserTracker(user2));

		var queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
		await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", Array.Empty<Instrument>(), db, tracker);

		user1.Signups.ShouldBeEmpty();
		queue = db.GetQueuedSongs();
		queue.Select(pair => pair.Song).ShouldContain(hurt);
	}


	[Fact]
	public async Task Song_Updates_User_Name_In_Database() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		var instruments = new[] { Instrument.Sing, Instrument.BassGuitar };
		db.SaveUser(user);
		var tracker = new FakeUserTracker(user);
		await SongEndpoints.SignUpForSong(hurt.Slug, "Surly Dev", instruments, db, tracker);
		db.FindUser(user.Guid)!.Name.ShouldBe("Surly Dev");
	}

}
