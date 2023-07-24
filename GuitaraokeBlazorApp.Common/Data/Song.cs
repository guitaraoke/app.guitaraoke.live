using System.Text.RegularExpressions;

namespace GuitaraokeBlazorApp.Common.Data;

public partial class Song {

	public Song(string artist, string title) {
		Artist = artist;
		Title = title;
	}

	private const string SEPARATOR = " - ";
	public string Name => $"{Artist}{SEPARATOR}{Title}";

	public string Artist { get; set; }
	public string Title { get; set; }

	public string Slug
		=> slugRegex
			.Replace($"{Artist}-{Title}", "-")
			.ToLowerInvariant()
			.Trim('-');

	public bool Played => PlayedAt.HasValue;
	public DateTime? PlayedAt { get; set; }

	[GeneratedRegex("[^\\p{L}0-9]+")]
	private static partial Regex SlugRegex();
	private static readonly Regex slugRegex = SlugRegex();
}
