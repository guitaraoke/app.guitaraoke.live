namespace GuitaraokeWebApp.Data;

public class SongDatabase : ISongDatabase {
	private readonly IList<Song> songs;
	private readonly Dictionary<Guid, User> users = new();
	private readonly Dictionary<User, List<Song>> stars = new();

	public SongDatabase(IEnumerable<Song> songs) {
		this.songs = songs.ToList();
	}

	public IEnumerable<Song> ListSongs() => songs;

	public IEnumerable<Song> ListStarredSongs(User user)
		=> stars.GetValueOrDefault(user) ?? new();

	public bool ToggleStar(User user, Song song) {
		if (stars.ContainsKey(user)) {
			if (stars[user].Contains(song)) {
				stars[user].Remove(song);
				return false;
			}

			stars[user].Add(song);
		} else {
			stars.Add(user, new() { song });
		}

		return true;
	}

	public Song? FindSong(string slug)
		=> songs.FirstOrDefault(s => s.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase));

	public User? FindUser(Guid guid) => users.GetValueOrDefault(guid);
	public User SaveUser(User user) => users[user.Guid] = user;

	public Dictionary<Song, List<User>> ListStarredSongs()
		=> stars.SelectMany(pair => pair.Value.Select(song => (pair.Key, song)))
			.GroupBy(item => item.Item2)
			.ToDictionary(group => group.Key, group => group.Select(g => g.Item1).ToList());

	public void SignUp(User user, Song song, Instrument[] instruments) {
		user.Signups[song] = instruments;
	}

	public SongSelection FindSongForUser(Song song, User user) {
		return new(song) {
			User = user,
			IsStarred = ListStarredSongs(user).Contains(song),
			Instruments = user.Signups.GetValueOrDefault(song) ?? new Instrument[] { }
		};
	}
}
