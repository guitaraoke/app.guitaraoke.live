namespace GuitaraokeWebApp.Model; 

public class Song {

	public Song(string artist, string title) {
		Artist = artist;
		Title = title;
	}

	public string Artist { get; set; }
	public string Title { get; set; }
}

public class SongSelection {
	public SongSelection(Song song) {
		Song = song;
	}
	public Song Song { get; set; }
	public bool IsStarred { get; set; }
	public override string ToString() => $"[{(IsStarred ? "*" : " ")}] {Song.Artist} - {Song.Title}";
}