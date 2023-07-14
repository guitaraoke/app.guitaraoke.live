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

	private readonly List<Song> queuedSongs = new();

	public List<(Song Song, Dictionary<User, Instrument[]> Players)> GetQueuedSongs()
		=> queuedSongs.Select(song => (song, ListPlayers(song))).ToList();

	public Dictionary<Song, int> GetStarredSongs()
		=> ListStarredSongs().ToDictionary(pair => pair.Key, pair => pair.Value.Count);

	public void AddSongToQueue(Song song) {
		if (queuedSongs.Contains(song)) return;
		queuedSongs.Add(song);
	}

	public void MoveSongToPosition(Song song, int newIndex) {
		if (queuedSongs.Contains(song)) queuedSongs.Remove(song);
		if (newIndex < 0) newIndex = 0;
		if (newIndex >= queuedSongs.Count) newIndex = queuedSongs.Count;
		queuedSongs.Insert(newIndex, song);
	}

	public void RemoveSongFromQueue(Song song)
		=> queuedSongs.Remove(song);

	private Dictionary<User, Instrument[]> ListPlayers(Song song)
		=> users.Values
			.Where(user => user.Signups.ContainsKey(song))
			.ToDictionary(user => user, user => user.Signups[song]);



	public Dictionary<Song, List<User>> ListStarredSongs()
		=> stars.SelectMany(pair => pair.Value.Select(song => (pair.Key, song)))
			.GroupBy(item => item.Item2)
			.ToDictionary(group => group.Key, group => group.Select(g => g.Item1).ToList());

	public SongSelection FindSongForUser(Song song, User user) {
		return new(song) {
			User = user,
			IsStarred = ListStarredSongs(user).Contains(song)
		};
	}

	public void PopulateSampleData() {
		var alicia = new User { Name = "Alicia Keys" };
		var ben = new User { Name = "Ben Folds" };
		var chris = new User { Name = "Chris Catalyst" };
		var david = new User { Name = "David Coverdale" };
		var eddie = new User { Name = "Eddie van Halen" };
		var freddie = new User { Name = "Freddie Mercury" };

		SaveUser(alicia);
		SaveUser(ben);
		SaveUser(chris);
		SaveUser(david);
		SaveUser(eddie);
		SaveUser(freddie);
		var testUsers = new[] { alicia, ben, chris, david, eddie, freddie };

		var instruments = Enum.GetValues<Instrument>().ToArray();
		for (var i = 0; i < 50; i++) {
			var song = this.songs[Random.Shared.Next(this.songs.Count)];
			var user = testUsers[Random.Shared.Next(testUsers.Length)];
			var instrument = instruments[Random.Shared.Next(instruments.Length)];
			user.SignUp(song, instrument);
			AddSongToQueue(song);

		}

		for (var i = 0; i < 250; i++) {
			var song = this.songs[Random.Shared.Next(this.songs.Count)];
			var user = testUsers[Random.Shared.Next(testUsers.Length)];
			ToggleStar(user, song);
		}
	}
}
