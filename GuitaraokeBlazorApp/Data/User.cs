namespace GuitaraokeBlazorApp.Data;

[Flags]
public enum Instrument {
	[Description("Sing")]
	Sing,
	[Description("Lead Guitar")]
	LeadGuitar,
	[Description("Rhythm Guitar")]
	RhythmGuitar,
	[Description("Bass")]
	BassGuitar,
	[Description("Piano")]
	Piano,
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

	public bool SignUp(Song song, params Instrument[] instruments) {
		if (instruments.Any()) {
			Signups[song] = instruments;
			return true;
		}
		Signups.Remove(song);
		return false;
	}
}