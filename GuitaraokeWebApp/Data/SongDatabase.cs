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

	public Dictionary<Song, List<Guid>> ListStarredSongs() {
		// stars[guid] is a list of starred songs indexed by user
		// we need to pivot that into a list of starring users indexed by song

		// Go from:
		// stars[bob] = song1, song2, song3
		// stars[alice] = song1, song4, song5

		var faves = new List<(Guid,Song)>();
		foreach (var pair in stars) {
			faves.AddRange(pair.Value.Select(song => (pair.Key, song)));
		}

		var result = faves.GroupBy(item => item.Item2)
			.ToDictionary(group => group.Key, group => group.Select(g => g.Item1).ToList());

		// To this:
		// result[song1] = bob, alice
		// result[song2] = bob
		// result[song3] = bob
		return result;
	}
}