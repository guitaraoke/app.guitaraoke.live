using AngleSharp.Dom;
using GuitaraokeWebApp.Model;
using GuitaraokeWebApp.TagHelpers;

namespace GuitaraokeWebApp.Tests;

public class WebTests : IClassFixture<WebApplicationFactory<Program>> {
	private readonly WebApplicationFactory<Program> factory = new();

	[Fact]
	public async Task Root_Index_Returns_OK() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		response.IsSuccessStatusCode.ShouldBe(true);
	}

	private async Task<(Song, HttpResponseMessage)> GetSong() {
		var client = factory.CreateClient();
		var db = factory.Services.GetService<ISongDatabase>();
		var song = db.ListSongs().First(s => s.Title.Contains("'"));
		var response = await client.GetAsync($"/song/{song.Slug}");
		return (song, response);
	}

	[Fact]
	public async Task Root_Song_Returns_Song() {
		var (_, response) = await GetSong();
		response.IsSuccessStatusCode.ShouldBe(true);
	}

	private async Task<(Song, IDocument)> GetSongDocument() {
		var (song, response) = await GetSong();
		var html = await response.Content.ReadAsStringAsync();
		var context = BrowsingContext.New(Configuration.Default);
		var document = await context.OpenAsync(req => req.Content(html));
		return (song, document);
	}

	[Fact]
	public async Task Root_Song_Includes_Song_Details() {
		var (song, document) = await GetSongDocument();
		var h1 = document.QuerySelector("h1");
		h1.InnerHtml.ShouldBe(song.Title);
		var h2 = document.QuerySelector("h2");
		h2.InnerHtml.ShouldBe(song.Artist);
	}

	[Fact]
	public async Task Root_Song_Includes_Song_Title() {
		var (song, document) = await GetSongDocument();
		var title = document.QuerySelector("title");
		title.InnerHtml.ShouldBe(song.Name);
	}

	[Fact]
	public async Task Root_Index_Includes_Songs() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		var html = await response.Content.ReadAsStringAsync();
		var decodedHtml = WebUtility.HtmlDecode(html);
		var db = factory.Services.GetService<ISongDatabase>();
		foreach (var song in db.ListSongs()) {
			decodedHtml.ShouldContain(song.Title);
			decodedHtml.ShouldContain(song.Artist);
		}
	}

	[Fact]
	public async Task Root_Index_Includes_Starred_Songs() {
		var userGuid = Guid.NewGuid();
		var client = factory.CreateClient();
		var db = factory.Services.GetService<ISongDatabase>();
		var song = db.ListSongs().First();
		db!.ToggleStar(userGuid, song);
		var request = new HttpRequestMessage(HttpMethod.Get, "/");
		var cookie = new Cookie(HttpCookieUserTracker.COOKIE_NAME, userGuid.ToString());
		request.Headers.Add("Cookie", cookie.ToString());
		var response = await client.SendAsync(request);
		var html = await response.Content.ReadAsStringAsync();
		var context = BrowsingContext.New(Configuration.Default);
		var document = await context.OpenAsync(req => req.Content(html));
		var elements = document.QuerySelectorAll($"li.song a.{SongStarTagHelper.STARRED}");
		var slug = elements.Single().GetAttribute(SongStarTagHelper.DATA_ATTRIBUTE_NAME);
		slug.ShouldBe(song.Slug);
	}

	[Fact]
	public async Task Root_Index_Sets_UserGuid_Cookie() {
		var response = await factory.CreateClient().GetAsync("/");
		var cookies = response.Headers.GetValues("Set-Cookie");
		var cookie = cookies.Single(c => c.StartsWith(HttpCookieUserTracker.COOKIE_NAME));
		var tokens = cookie.Split(";")[0].Split("=");
		tokens[0].ShouldBe(HttpCookieUserTracker.COOKIE_NAME);
		Guid.TryParse(tokens[1], out var guid).ShouldBe(true);
		guid.ShouldNotBe(default);
	}
}

