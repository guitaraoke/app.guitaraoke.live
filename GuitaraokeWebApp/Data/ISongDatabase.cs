using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Data {
	public interface ISongDatabase {
		IEnumerable<Song> ListSongs();
		IEnumerable<Song> ListStarredSongs(Guid userGuid);
		void ToggleStar(Guid userGuid, Song song);
	}
}