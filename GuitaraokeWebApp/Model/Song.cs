namespace GuitaraokeWebApp.Model;

public class SongQueue {
	public Dictionary<Song, int> StarredSongs { get; set; } = new();
}

public class Song {

	public Song(string artist, string title) {
		Artist = artist;
		Title = title;
	}

	private const string SEPARATOR = " - ";
	public string Name => $"{Artist}{SEPARATOR}{Title}";

	public string Artist { get; set; }
	public string Title { get; set; }
	public static System.Text.RegularExpressions.Regex regex = new("[^\\p{L}0-9]+");

	public string Slug
		=> regex.Replace($"{Artist}-{Title}", "-").ToLowerInvariant().Trim('-');
}