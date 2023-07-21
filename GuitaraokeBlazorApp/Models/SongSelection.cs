using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuitaraokeBlazorApp.Model;

public class SongSelection {
	public SongSelection(Song song) {
		Song = song;
	}
	public User User { get; set; }
	public Song Song { get; set; }
	public bool IsStarred { get; set; }
	public override string ToString() => $"[{(IsStarred ? "*" : " ")}] {Song.Artist} - {Song.Title}";

	public IEnumerable<SelectListItem> InstrumentOptions =>
		Enum
			.GetValues<Instrument>()
			.Select(item => new SelectListItem {
				Value = item.ToString(),
				Text = item.ToString(),
				Selected = this.Instruments.Contains(item)
			});

	public Instrument[] Instruments =>
		this.User.Signups.GetValueOrDefault(Song) ?? Array.Empty<Instrument>();
}
