namespace GuitaraokeBlazorApp.Endpoints;

public static partial class SongEndpoints {

	public static void MapSongEndpoints(this WebApplication? app) {
		app?.MapPost("/star/{slug}", ToggleStar);
		app?.MapPost("/song/{slug}", SignUpForSong);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public static async Task<IResult> ToggleStar(string slug, ISongDatabase db, IUserTracker tracker) {
		var song = db.FindSong(slug);
		if (song == default) return Results.NotFound();
		var user = tracker.GetUser();
		return Results.Ok(db.ToggleStar(song, user));
	}

	public static async Task<IResult> SignUpForSong(string slug, string name, Instrument[] instruments, ISongDatabase db, IUserTracker tracker) {
		var user = tracker.GetUser();
		var song = db.FindSong(slug);
		if (song == null) return Results.NotFound();
		user.Name = name;
		db.SaveUser(user);
		db.SignUpForSong(song, user, instruments);
		return Results.Ok();
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

}
