namespace GuitaraokeWebApp.Data;

public class User {
	public User() { }
	public User(Guid guid) {
		Guid = guid;
	}

	public Guid Guid { get; set; } = Guid.NewGuid();
}