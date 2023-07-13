using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Tests.Data {
	public class SongDatabaseTests {

		[Fact]
		public void ListStarredSongs_Returns_Songs_With_Stars() {
			var a = new Song("a", "a");
			var b = new Song("b", "b");
			var c = new Song("c", "c");
			var db = new SongDatabase(new[] { a, b, c });
			db.ToggleStar(Guid.NewGuid(), a);
			db.ToggleStar(Guid.NewGuid(), a);
			db.ToggleStar(Guid.NewGuid(), a);
			db.ToggleStar(Guid.NewGuid(), b);
			db.ToggleStar(Guid.NewGuid(), b);
			db.ToggleStar(Guid.NewGuid(), c);
			var result = db.ListStarredSongs();
			result[a].Count.ShouldBe(3);
			result[b].Count.ShouldBe(2);
			result[c].Count.ShouldBe(1);
		}

		[Fact]
		public void ToggleStar_Toggles_Star_For_First_Song() {
			var humanLove = new Song("Chroma Key", "Human Love");
			var oneSecond = new Song("Napalm Death", "One Second");
			var userGuid = Guid.NewGuid();
			var db = new SongDatabase(new [] { humanLove, oneSecond });
			db.ListStarredSongs(userGuid).ShouldBeEmpty();

			db.ToggleStar(userGuid, humanLove);
			db.ListStarredSongs(userGuid).Single().ShouldBe(humanLove);

			db.ToggleStar(userGuid, oneSecond);
			var list = db.ListStarredSongs(userGuid).ToList();
			list.Count.ShouldBe(2);
			list.ShouldContain(oneSecond);
			list.ShouldContain(humanLove);

			db.ToggleStar(userGuid, humanLove);
			db.ListStarredSongs(userGuid).Single().ShouldBe(oneSecond);

			db.ToggleStar(userGuid, oneSecond);
			db.ListStarredSongs(userGuid).ShouldBeEmpty();

		}
	}
}
