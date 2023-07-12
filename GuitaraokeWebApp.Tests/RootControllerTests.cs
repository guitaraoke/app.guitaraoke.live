using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests;

public class RootControllerTests {
	private readonly List<Song> songs;
	
	public RootControllerTests() {
		songs = new() { new("Abba", "Waterloo") };
	}

	[Fact]
	public async Task Index_Returns_SongList() {
		var db = new SongDatabase(songs);
		var c = new RootController(new NullLogger<RootController>(), db, new FakeUserTracker());
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = (result.Model as IEnumerable<SongSelection>).ToList();
		model.ShouldNotBeNull();
		model.ShouldNotBeEmpty();
	}

	[Fact]
	public async Task Index_Shows_Starred_Songs_For_User() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once });
		var guid = Guid.NewGuid();
		db.ToggleStar(guid, hurt);
		var tracker = new FakeUserTracker(guid);
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
		var model = (result.Model as IEnumerable<SongSelection>).ToList();
		foreach (var song in model) song.IsStarred.ShouldBe(false);
	}
}