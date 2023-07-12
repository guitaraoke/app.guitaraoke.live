namespace GuitaraokeWebApp.Tests;

public class FakeUserTracker : IUserTracker {
	private readonly Guid guid;
	public FakeUserTracker(Guid guid = default) {
		this.guid = guid;
	}
	public Guid GetUserGuid() => guid;
}