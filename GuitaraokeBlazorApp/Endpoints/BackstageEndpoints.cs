using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeBlazorApp.Endpoints;

public static partial class BackstageEndpoints {
	public static void MapBackstageEndpoints(this WebApplication? app) {
		app?.MapPost("/backstage/move", Move);
		app?.MapPost("/backstage/status/{slug}", Status);
	}

	public class MoveSongData {
		public required string Song { get; set; }
		public required int Position { get; set; }
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public static async Task<IResult> Move([FromBody] MoveSongData post, ISongDatabase db) {
		var song = db.FindSong(post.Song);
		if (song == default) return Results.BadRequest();
		db.MoveSongToPosition(song, post.Position);
		return Results.Ok();
	}

	public static async Task<IResult> Status(string slug, bool played, ISongDatabase db) {
	var song = db.FindSong(slug);
		if (song == default) return Results.NotFound();
		song.PlayedAt = (played ? DateTime.UtcNow : null);
		if (song.Played) db.RemoveSongFromQueue(song);
		return Results.Ok(played);
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously


}
