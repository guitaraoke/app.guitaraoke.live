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
		var c = new RootController(new NullLogger<RootController>(), db);
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = (result.Model as IEnumerable<SongSelection>).ToList();
		model.ShouldNotBeNull();
		model.ShouldNotBeEmpty();
	}

	//[Fact]
	//public async Task Index_Shows_Starred_Songs_For_User() {
	//	var db = new SongDatabase(songs);
		

	//}

	[Fact]
	public async Task Index_For_New_User_Has_No_Stars() {
		var db = new SongDatabase(songs);
		var c = new RootController(new NullLogger<RootController>(), db);
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = (result.Model as IEnumerable<SongSelection>).ToList();
		foreach (var song in model) song.IsStarred.ShouldBe(false);
	}
}