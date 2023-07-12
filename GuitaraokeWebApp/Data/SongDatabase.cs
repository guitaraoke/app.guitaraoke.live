using GuitaraokeWebApp.Model;

namespace GuitaraokeWebApp.Data;

public class SongDatabase : ISongDatabase {
	private readonly IList<Song> songs;
	private readonly Dictionary<Guid, List<Song>> stars;

	public SongDatabase(IEnumerable<Song> songs) {
		this.songs = songs.ToList();
		this.stars = new();
	}

	public IEnumerable<Song> ListSongs() => songs;

	public IEnumerable<Song> ListStarredSongs(Guid userGuid)
		=> stars.GetValueOrDefault(userGuid) ?? new();

	public bool ToggleStar(Guid userGuid, Song song) {
		if (stars.ContainsKey(userGuid)) {
			if (stars[userGuid].Contains(song)) {
				stars[userGuid].Remove(song);
				return false;
			}
			stars[userGuid].Add(song);
		}
		else {
			stars.Add(userGuid, new() { song });
		}
		return true;
	}

	public Song FindSong(string slug)
		=> songs.FirstOrDefault(s => s.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase));
}