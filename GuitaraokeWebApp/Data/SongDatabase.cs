using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Data;

public class SongDatabase : ISongDatabase {
	private readonly IList<Song> songs;

	public SongDatabase(IEnumerable<Song> songs) {
		this.songs = songs.ToList();
	}

	public IEnumerable<Song> ListSongs() => songs;
}