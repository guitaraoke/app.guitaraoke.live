using GuitaraokeBlazorApp.Components;

namespace GuitaraokeBlazorApp.Data;

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
		if (stars.TryGetValue(user, out var value)) {
			if (value.Contains(song)) {
				value.Remove(song);
				return false;
			}

			value.Add(song);
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

	public void PruneQueue() => queuedSongs.RemoveAll(queuedSong => ListPlayers(queuedSong).Count == 0);

	private Dictionary<User, Instrument[]> ListPlayers(Song song)
		=> users.Values
			.Where(user => user.Signups.ContainsKey(song))
			.ToDictionary(user => user, user => user.Signups[song]);



	public Dictionary<Song, List<User>> ListStarredSongs()
		=> stars.SelectMany(pair => pair.Value.Select(song => (pair.Key, song)))
			.GroupBy(item => item.song)
			.ToDictionary(group => group.Key, group => group.Select(g => g.Key).ToList());

	public Instrument[] FindInstrumentsForUser(Song song, User user)
		=> user.Signups.GetValueOrDefault(song) ?? Array.Empty<Instrument>();

	//public SongSelection FindSongForUser(Song song, User user) {
	//	return new(song) {
	//		User = user,
	//		IsStarred = ListStarredSongs(user).Contains(song)
	//	};
	//}

	public void PopulateSampleData() {
		var alicia = new User { Name = "Alicia" };
		var ben = new User { Name = "Ben F" };
		var chris = new User { Name = "Chris C" };
		var david = new User { Name = "David C" };
		var eddie = new User { Name = "Ed van H" };
		var freddie = new User { Name = "Freddie M" };
		var gloria = new User { Name = "Gloria"};
		var harry = new User { Name = "Harry" };
		var iggy = new User { Name = "I. Pop" };
		var joe = new User { Name = "Joe B" };
		var kerry = new User { Name = "Kerry" };
		var liam = new User { Name = "Liam" };
		var mike = new User { Name = "Mike" };

		SaveUser(alicia);
		SaveUser(ben);
		SaveUser(chris);
		SaveUser(david);
		SaveUser(eddie);
		SaveUser(freddie);
		SaveUser(gloria);
		SaveUser(harry);
		SaveUser(iggy);
		SaveUser(joe);
		SaveUser(kerry);
		SaveUser(liam);
		SaveUser(mike);

		var testUsers = new[] { alicia, ben, chris, david, eddie, freddie, gloria, harry, iggy, joe, kerry, liam, mike };

		var instruments = Enum.GetValues<Instrument>().ToArray();
		for (var i = 0; i < 50; i++) {
			var song = this.songs[Random.Shared.Next(this.songs.Count)];
			for (var j = 0; j < 2 + Random.Shared.Next(3); j++) {
				var user = testUsers[Random.Shared.Next(testUsers.Length)];
				var instrument = instruments[Random.Shared.Next(instruments.Length)];
				user.SignUp(song, instrument);
			}
			AddSongToQueue(song);
		}

		for (var i = 0; i < 1000; i++) {
			var song = this.songs[Random.Shared.Next(this.songs.Count)];
			var user = testUsers[Random.Shared.Next(testUsers.Length)];
			ToggleStar(user, song);
		}
	}
}
