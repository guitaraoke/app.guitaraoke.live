using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GuitaraokeBlazorApp.TagHelpers; 

public class SongStarTagHelper : TagHelper {
	public const string STARRED = "fa-solid";
	public const string NORMAL = "fa-thin";
	public const string DATA_ATTRIBUTE_NAME = "data-song-slug";
	public SongSelection? Selection { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output) {
		output.TagName = "a";
		output.TagMode = TagMode.StartTagAndEndTag;
		var faStyle = Selection.IsStarred ? STARRED: NORMAL;
		var classAttributeValue = $"song-star fa-star {faStyle}";
		output.Attributes.SetAttribute("class", classAttributeValue);
		output.Attributes.SetAttribute(DATA_ATTRIBUTE_NAME, Selection.Song.Slug);
	}
}
