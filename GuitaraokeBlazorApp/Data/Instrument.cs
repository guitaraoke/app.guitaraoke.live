using System.Reflection;

namespace GuitaraokeBlazorApp.Data;

[Flags]
public enum Instrument {
	[Description("Sing")]
	Sing,
	[Description("Lead Guitar")]
	LeadGuitar,
	[Description("Rhythm Guitar")]
	RhythmGuitar,
	[Description("Bass")]
	BassGuitar,
	[Description("Piano")]
	Piano
}

public static class EnumExtensions {
	public static string GetDisplayName(this Enum enumValue) => enumValue.GetType().GetMember(enumValue.ToString())?
		.FirstOrDefault()?
			.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();
}

