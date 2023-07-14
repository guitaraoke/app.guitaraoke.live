﻿using AngleSharp.Dom;
using GuitaraokeWebApp.Model;
using GuitaraokeWebApp.TagHelpers;

namespace GuitaraokeWebApp.Tests;

public class WebTests : IClassFixture<WebApplicationFactory<Program>> {
	private readonly WebApplicationFactory<Program> factory = new();
	private ISongDatabase db => factory.Services.GetService<ISongDatabase>()!;
	private IBrowsingContext angleSharp => BrowsingContext.New(Configuration.Default);

	[Fact]
	public async Task Root_Index_Returns_OK() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		response.IsSuccessStatusCode.ShouldBe(true);
	}

	private async Task<(Song, HttpResponseMessage)> GetSong() {
		var client = factory.CreateClient();
		var song = db!.ListSongs().First(s => s.Title.Contains("'"));
		var response = await client.GetAsync($"/song/{song.Slug}");
		return (song, response);
	}

	[Fact]
	public async Task Root_Queue_Returns_View() {
		var client = factory.CreateClient();
		var result = await client.GetAsync("/queue");
		result.IsSuccessStatusCode.ShouldBe(true);
	}

	[Fact]
	public async Task Root_Song_Returns_Song() {
		var (_, response) = await GetSong();
		response.IsSuccessStatusCode.ShouldBe(true);
	}

	private async Task<(Song, IDocument)> GetSongDocument() {
		var (song, response) = await GetSong();
		var html = await response.Content.ReadAsStringAsync();
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		return (song, document);
	}

	[Fact]
	public async Task Root_Song_Includes_Song_Details() {
		var (song, document) = await GetSongDocument();
		var h1 = document.QuerySelector("h1");
		h1!.InnerHtml.ShouldBe(song.Title);
		var h2 = document.QuerySelector("h2");
		h2!.InnerHtml.ShouldBe(song.Artist);
	}

	[Fact]
	public async Task Root_Song_Includes_Song_Title() {
		var (song, document) = await GetSongDocument();
		var title = document.QuerySelector("title");
		title!.InnerHtml.ShouldBe(song.Name);
	}

	[Theory]
	[InlineData("Björk")]
	[InlineData("Dolores O'Riordan")]
	[InlineData("Святослав Іванович Вакарчук")]
	[InlineData("💩")]
	public async Task Signup_Form_Includes_Name(string name) {
		var user = new User { Name = name };
		db.SaveUser(user);
		var song = db.ListSongs().First();
		var html = await GetHtmlForPage($"/song/{song.Slug}", user);
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		var input = document.QuerySelector("input[name=name]")!;
		input.GetAttribute("value").ShouldBe(name);
	}

	[Fact]
	public async Task Root_Index_Includes_Songs() {
		var client = factory.CreateClient();
		var response = await client.GetAsync("/");
		var html = await response.Content.ReadAsStringAsync();
		var decodedHtml = WebUtility.HtmlDecode(html);
		foreach (var song in db!.ListSongs()) {
			decodedHtml.ShouldContain(song.Title);
			decodedHtml.ShouldContain(song.Artist);
		}
	}

	[Fact]
	public async Task Root_Index_Includes_Starred_Songs() {
		var user = new User();
		var client = factory.CreateClient();
		db.SaveUser(user);
		var song = db!.ListSongs().First();
		db!.ToggleStar(user, song);
		var request = new HttpRequestMessage(HttpMethod.Get, "/");
		var cookie = new Cookie(HttpCookieUserTracker.COOKIE_NAME, user.Guid.ToString());
		request.Headers.Add("Cookie", cookie.ToString());
		var response = await client.SendAsync(request);
		var html = await response.Content.ReadAsStringAsync();
		
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		var elements = document.QuerySelectorAll($"li.song a.{SongStarTagHelper.STARRED}");
		var slug = elements.Single().GetAttribute(SongStarTagHelper.DATA_ATTRIBUTE_NAME);
		slug.ShouldBe(song.Slug);
	}

	[Fact]
	public async Task Root_Me_Returns_Ok() {
		var response = await factory.CreateClient().GetAsync("/me");
		var html = await response.Content.ReadAsStringAsync();
		
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		document.ShouldNotBeNull();
		document.QuerySelector("title")!.InnerHtml.ShouldBe("My Songs");
	}

	[Theory]
	[InlineData("Surly Dev", "Hi Surly Dev")]
	[InlineData("Dylan", "Hi Dylan")]
	[InlineData(null, "Hi there")]
	[InlineData("", "Hi ")]
	public async Task Root_Me_Includes_Name(string name, string expectedHtml) {
		var user = new User { Name = name };
		var html = await GetHtmlForMePage(user);
		
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		document.ShouldNotBeNull();
		document.QuerySelector("h1")!.InnerHtml.ShouldBe(expectedHtml);
	}

	[Fact]
	public async Task Root_Me_With_No_Signups_Returns_Message() {
		var user = new User { Name = "Eddie" };
		var html = await GetHtmlForMePage(user);
		
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		document.ShouldNotBeNull();
		document.QuerySelector(".no-signups-message").ShouldNotBeNull();
	}

	[Fact]
	public async Task Root_Me_With_Signups_Includes_Song_List() {
		var song = db.ListSongs().First();
		var user = new User { Name = "Geddy" };
		user.SignUp(song, new[] { Instrument.Sing, Instrument.BassGuitar });
		db.SaveUser(user);
		var html = await GetHtmlForMePage(user);
		
		var document = await angleSharp.OpenAsync(req => req.Content(html));
		document.ShouldNotBeNull();
		document.QuerySelector(".no-signups-message").ShouldBeNull();
		var liHtml = document.QuerySelectorAll(".signup-list li").Single().InnerHtml;
		liHtml.ShouldContain(song.Title);
		liHtml.ShouldContain(song.Artist);
		liHtml.ShouldContain(song.Slug);
	}

	private async Task<string> GetHtmlForMePage(User user)
		=> await GetHtmlForPage("/me", user);

	private async Task<string> GetHtmlForPage(string url, User user) {
		var client = factory.CreateClient();
		db.SaveUser(user);
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		var cookie = new Cookie(HttpCookieUserTracker.COOKIE_NAME, user.Guid.ToString());
		request.Headers.Add("Cookie", cookie.ToString());
		var response = await client.SendAsync(request);
		var html = await response.Content.ReadAsStringAsync();
		return html;
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

