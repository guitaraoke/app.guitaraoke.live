namespace GuitaraokeWebApp.Data {
	public interface ISongDatabase {
		IEnumerable<Song> ListSongs();
	}

	public class SongDatabase : ISongDatabase {
		private readonly IList<Song> songs;

		public SongDatabase(IEnumerable<Song> songs) {
			this.songs = songs.ToList();
		}

		public IEnumerable<Song> ListSongs() => songs;
	}
}

public class Song {

	public Song(string artist, string title) {
		Artist = artist;
		Title = title;
	}

	public string Artist { get; set; }
	public string Title { get; set; }
}
