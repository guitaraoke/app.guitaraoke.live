namespace GuitaraokeBlazorApp.Tests.Data;

public class SongDatabaseTests {

	[Fact]
	public void ListStarredSongs_Returns_Songs_With_Stars() {
		var a = new Song("a", "a");
		var b = new Song("b", "b");
		var c = new Song("c", "c");
		var db = new SongDatabase(new[] { a, b, c });
		db.ToggleStar(a, new());
		db.ToggleStar(a, new());
		db.ToggleStar(a, new());
		db.ToggleStar(b, new());
		db.ToggleStar(b, new());
		db.ToggleStar(c, new());
		var result = db.ListStarredSongs();
		result[a].Count.ShouldBe(3);
		result[b].Count.ShouldBe(2);
		result[c].Count.ShouldBe(1);
	}

	[Fact]
	public void ToggleStar_Toggles_Star_For_First_Song() {
		var humanLove = new Song("Chroma Key", "Human Love");
		var oneSecond = new Song("Napalm Death", "One Second");
		var user = new User();
		var db = new SongDatabase(new[] { humanLove, oneSecond });
		db.ListStarredSongs(user).ShouldBeEmpty();

		db.ToggleStar(humanLove, user);
		db.ListStarredSongs(user).Single().ShouldBe(humanLove);

		db.ToggleStar(oneSecond, user);
		var list = db.ListStarredSongs(user).ToList();
		list.Count.ShouldBe(2);
		list.ShouldContain(oneSecond);
		list.ShouldContain(humanLove);

		db.ToggleStar(humanLove, user);
		db.ListStarredSongs(user).Single().ShouldBe(oneSecond);

		db.ToggleStar(oneSecond, user );
		db.ListStarredSongs(user).ShouldBeEmpty();

	}
}
