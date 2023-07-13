namespace GuitaraokeWebApp.Tests;

public class FakeUserTracker : IUserTracker {
	private readonly User user;

	public FakeUserTracker(User? user = default) {
		this.user = user ??= new();
	}
	public User GetUser() => user;
}