using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Data {
	public interface ISongDatabase {
		IEnumerable<Song> ListSongs();
	}
}