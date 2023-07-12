using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Data; 

public interface ISongDatabase {
	IEnumerable<Song> ListSongs();
	IEnumerable<Song> ListStarredSongs(Guid userGuid);
	/// <summary>Toggle the specified song star for the specified user.</summary>
	/// <returns>True if the song is now starred; otherwise false.</returns>
	bool ToggleStar(Guid userGuid, Song song);
	Song FindSong(string slug);
}