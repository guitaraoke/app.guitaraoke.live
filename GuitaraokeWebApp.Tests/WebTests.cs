namespace GuitaraokeWebApp.Tests;

public class WebTests : IClassFixture<WebApplicationFactory<Program>> {
	private readonly WebApplicationFactory<Program> factory = new();
	[Fact]
	public async Task Root_Index_Returns_OK() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		response.IsSuccessStatusCode.ShouldBe(true);
	}

	[Fact]
	public async Task Root_Index_Includes_Songs() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		var pageHtml = await response.Content.ReadAsStringAsync();
		var db = factory.Services.GetService<ISongDatabase>();
		foreach (var song in db.ListSongs()) {
			pageHtml.ShouldContain(song.Title);
			pageHtml.ShouldContain(song.Artist);
		}
	}
}