using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Microsoft.OpenApi.Attributes;

namespace GuitaraokeWebApp.Model;

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
			// TAKE OUT BASS FOR NDC PORTO
			.Where(item => item != Instrument.BassGuitar)
			.Select(item => new SelectListItem {
				Value = item.ToString(),
				Text = item.GetDisplayName(),
				Selected = this.Instruments.Contains(item)
			});

	public Instrument[] Instruments =>
		this.User.Signups.GetValueOrDefault(Song) ?? new Instrument[] { };
}

public static class EnumExtensions {
	public static string GetDisplayName(this Enum enumValue) => enumValue.GetType().GetMember(enumValue.ToString())?
		.FirstOrDefault()?
			.GetCustomAttribute<DisplayAttribute>()?.Name ?? enumValue.ToString();
}

