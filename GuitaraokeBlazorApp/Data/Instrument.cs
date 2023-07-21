using System.Reflection;
using System.Runtime.InteropServices;

namespace GuitaraokeBlazorApp.Data;

[Flags]
public enum Instrument {
	
	[Description("Sing")]          Sing         =  1,
	[Description("Lead Guitar")]   LeadGuitar   =  2,
	[Description("Rhythm Guitar")] RhythmGuitar =  4,
	[Description("Bass")]          BassGuitar   =  8,
	[Description("Piano")]         Piano        = 16
}

public static class EnumExtensions {
	public static string GetDisplayName(this Enum enumValue) => enumValue.GetType().GetMember(enumValue.ToString())?
		.FirstOrDefault()?
		.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();

	public static Instrument[] ToArray(this Enum enumValue) => enumValue
		.ToString()
		.Split(",")
		.Select(str => Enum.Parse<Instrument>(str))
		.ToArray();

	public static Instrument ToInstrument(this Instrument[] enumArray) {
		if (enumArray is null || enumArray.Length < 1) {
			return 0;
		}
		return Enum.Parse<Instrument>(String.Join(",", enumArray.Select(s => s.ToString())));
	}
}

