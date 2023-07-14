namespace GuitaraokeWebApp.Data;

public interface ISongDatabase {
	IEnumerable<Song> ListSongs();
	Dictionary<Song, List<User>> ListStarredSongs();
	IEnumerable<Song> ListStarredSongs(User user);
	/// <summary>Toggle the specified song star for the specified user.</summary>
	/// <returns>True if the song is now starred; otherwise false.</returns>
	bool ToggleStar(User user, Song song);
	Song? FindSong(string slug);
	User? FindUser(Guid guid);
	User SaveUser(User user);
	List<(Song, Dictionary<User, Instrument[]>)> GetQueuedSongs();
	Dictionary<Song, int> GetStarredSongs();
	void AddSongToQueue(Song song);
}
