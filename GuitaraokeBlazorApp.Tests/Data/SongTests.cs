namespace GuitaraokeBlazorApp.Tests.Data;

public class SongTests {
	[Theory]
	[InlineData("AC/DC", "T.N.T.", "ac-dc-t-n-t")]
	[InlineData("AC/DC", "Ain't No Fun (Waiting Round to Be a Millionaire)",
		"ac-dc-ain-t-no-fun-waiting-round-to-be-a-millionaire")]
	[InlineData("BØRNS", "title", "børns-title")]
	[InlineData("CHVRCHΞS", "title", "chvrchξs-title")]
	[InlineData("Ke$ha", "title", "ke-ha-title")]
	public void Song_Slug_Works(string artist, string title, string slug)
		=> new Song(artist, title).Slug.ShouldBe(slug);
}
