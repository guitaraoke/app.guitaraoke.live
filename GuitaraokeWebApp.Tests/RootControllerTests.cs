namespace GuitaraokeWebApp.Tests;

public class RootControllerTests {
	private List<Song> songs;
	
	public RootControllerTests() {
		songs = new() { new("Abba", "Waterloo") };
	}

	[Fact]
	public async Task Index_Returns_SongList() {
		var db = new SongDatabase(songs);
		var c = new RootController(new NullLogger<RootController>(), db);
		var result = await c.Index() as ViewResult;
		result.ShouldNotBeNull();
		var model = (result.Model as IEnumerable<Song>).ToList();
		model.ShouldNotBeNull();
		model.ShouldNotBeEmpty();
	}
}