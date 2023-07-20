namespace GuitaraokeBlazorApp.Tests;

public class RootControllerIndexTests : RootControllerTestBase {

	[Fact]
	public async Task Index_Returns_SongList() {
		var db = new SongDatabase(songs);
		var c = MakeController();
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = ((result.Model as IEnumerable<SongSelection>)!).ToList();
		model.ShouldNotBeNull();
		model.ShouldNotBeEmpty();
	}

	[Fact]
	public async Task Index_Shows_Starred_Songs_For_User() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var user = new User();
		db.ToggleStar(user, hurt);
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker);
		var selection = ((await c.Index() as ViewResult)!.Model as IEnumerable<SongSelection>)!.ToList();
		selection.ShouldNotBeEmpty();
		selection.Single(s => s.Song == hurt).IsStarred.ShouldBe(true);
		selection.Where(s => s.Song != hurt).ShouldAllBe(sel => sel.IsStarred == false);
	}

	[Fact]
	public async Task Index_For_New_User_Has_No_Stars() {
		var db = new SongDatabase(songs);
		var c = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker());
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = ((result.Model as IEnumerable<SongSelection>)!).ToList();
		foreach (var song in model) song.IsStarred.ShouldBe(false);
	}

	[Fact]
	public async Task Starring_Song_Toggles_Star_On() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var user = new User();
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker);
		await c.Star(hurt.Slug);
		db.ListStarredSongs(user).Count().ShouldBe(1);
		await c.Star(hurt.Slug);
		db.ListStarredSongs(user).Count().ShouldBe(0);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("this-is-not-a-valid-song")]
	public async Task Starring_Song_With_Empty_Slug_Does_Not_Make_Everything_Break(string slug) {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var db = new SongDatabase(new[] { hurt });
		var user = new User();
		var tracker = new FakeUserTracker(user);
		var c = new RootController(new NullLogger<RootController>(), db, tracker);
		await c.Star(slug);
		var result = await c.Queue() as ViewResult;
		result.ShouldNotBeNull();
	}
}
