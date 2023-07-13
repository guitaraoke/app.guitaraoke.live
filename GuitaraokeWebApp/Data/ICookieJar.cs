namespace GuitaraokeWebApp.Data;

public interface ICookieJar {
	public string? Get(string name);
	public void Set(string name, string value);
}