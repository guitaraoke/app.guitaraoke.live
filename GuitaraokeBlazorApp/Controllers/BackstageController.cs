using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeBlazorApp.Controllers;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class BackstageController : Controller {
	private readonly ISongDatabase db;

	public BackstageController(ISongDatabase db) {
		this.db = db;
	}

	public async Task<IActionResult> Index() {
		var model = new SongQueue() {
			QueuedSongs = db.GetQueuedSongs(),
			StarredSongs = db.GetStarredSongs()
		};
		return View(model);
	}

	public async Task<IActionResult> Move([FromBody] MoveSongData post) {
		var song = db.FindSong(post.Song);
		if (song == default) return BadRequest();
		db.MoveSongToPosition(song, post.Position);
		return Json(true);
	}

	[HttpPost]
	public async Task<IActionResult> Status(string id, [FromBody] bool played) {
		var song = db.FindSong(id);
		if (song == default) return NotFound();
		song.PlayedAt = (played ? DateTime.UtcNow : null);
		if (song.Played) db.RemoveSongFromQueue(song);
		return Json(played);
	}
}

public class MoveSongData {
	public string Song { get; set; }
	public int Position { get; set; }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
