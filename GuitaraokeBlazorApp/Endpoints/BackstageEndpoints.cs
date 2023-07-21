using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeBlazorApp.Endpoints;

public static partial class BackstageEndpoints {
	public static void MapBackstageEndpoints(this WebApplication? app) {
		app?.MapGet("/backstage/move", Move);
		app?.MapPost("/backstage/status/{id}", Status);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public static async Task<IResult> Move([FromBody] MoveSongData post, ISongDatabase db, IUserTracker tracker) {
		var song = db.FindSong(post.Song);
		if (song == default) return Results.BadRequest();
		db.MoveSongToPosition(song, post.Position);
		return Results.Ok();
	}

	public static async Task<IResult> Status(string id, bool played, ISongDatabase db, IUserTracker tracker) {
		var song = db.FindSong(id);
		if (song == default) return Results.NotFound();
		song.PlayedAt = (played ? DateTime.UtcNow : null);
		if (song.Played) db.RemoveSongFromQueue(song);
		return Results.Ok(played);
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously


	public class MoveSongData {
		public string Song { get; set; } = default!;
		public int Position { get; set; }
	}

}
