namespace GuitaraokeBlazorApp.Endpoints;

public static partial class SongEndpoints {

	public static void MapSongEndpoints(this WebApplication? app) {
		app?.MapPost("/star/{id}", ToggleSongStar);
	}

	public static IResult ToggleSongStar(string id, ISongDatabase db, IUserTracker tracker) {
		var song = db.FindSong(id);
		if (song == default) return Results.NotFound();
		var user = tracker.GetUser();
		return Results.Ok(db.ToggleStar(user, song));
	}

}
