namespace GuitaraokeWebApp.Model;

public class SongSelection {
	public SongSelection(Song song) {
		Song = song;
	}
	public User User { get; set; }
	public Song Song { get; set; }
	public bool IsStarred { get; set; }
	public Instrument[] Instruments { get; set; } = { };
	public override string ToString() => $"[{(IsStarred ? "*" : " ")}] {Song.Artist} - {Song.Title}";
}