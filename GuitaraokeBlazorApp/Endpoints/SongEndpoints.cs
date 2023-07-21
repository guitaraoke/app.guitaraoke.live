namespace GuitaraokeBlazorApp.Endpoints;

public static partial class SongEndpoints {

	public static void MapSongEndpoints(this WebApplication? app) {
		app?.MapPost("/star/{id}", ToggleSongStar);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public static async Task<IResult> ToggleSongStar(string id, ISongDatabase db, IUserTracker tracker) {
		var song = db.FindSong(id);
		if (song == default) return Results.NotFound();
		var user = tracker.GetUser();
		return Results.Ok(db.ToggleStar(user, song));
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

}
