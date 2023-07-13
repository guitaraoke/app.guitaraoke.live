using System.Net.WebSockets;
using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests;

public class RootControllerTests {
	private readonly List<Song> songs;
	private readonly ISongDatabase db;

	private RootController MakeController() 
		=> new(new NullLogger<RootController>(), db, new FakeUserTracker());
	
	public RootControllerTests() {
		songs = new() { new("Abba", "Waterloo") };
		db = new SongDatabase(songs);
	}

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

	[Fact]
	public async Task Index_Returns_SongList() {
		var db = new SongDatabase(songs);
		var c = MakeController();
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

	[Fact]
	public async Task Starring_Song_Toggles_Star_On() {
		var hurt = new Song("Nine Inch Nails", "Hurt");
		var torn = new Song("Natalie Imbruglia", "Torn");
		var once = new Song("Pearl Jam", "Once");
		var db = new SongDatabase(new[] { hurt, torn, once }); var guid = Guid.NewGuid();

		var tracker = new FakeUserTracker(guid);
		var c = new RootController(new NullLogger<RootController>(), db, tracker);
		c.Star(hurt.Slug);
		db.ListStarredSongs(guid).Count().ShouldBe(1);
		c.Star(hurt.Slug);
		db.ListStarredSongs(guid).Count().ShouldBe(0);
	}
}