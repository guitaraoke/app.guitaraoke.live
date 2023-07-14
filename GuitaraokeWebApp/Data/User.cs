namespace GuitaraokeWebApp.Data;

[Flags]
public enum Instrument {
	Sing = 1 << 0,
	LeadGuitar = 1 << 1,
	RhythmGuitar = 1 << 2,
	BassGuitar = 1 << 3,
	Piano = 1 << 4,
	Theremin = 1 << 5
}

public class User {

	public string? Name { get; set; }

	public Guid Guid { get; set; }

	public User(Guid? guid = null, string? name = null) {
		Guid = guid ?? Guid.NewGuid();
		Name = name;

	}

	// When you sign up (two words), it creates a signup (one word)
	// Like how when you log in (two words), that's a login (one word)
	public Dictionary<Song, Instrument[]> Signups { get; set; } = new();

	public void SignUp(Song song, Instrument[] instruments) => Signups[song] = instruments;
}