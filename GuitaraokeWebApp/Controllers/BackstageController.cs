using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeWebApp.Controllers;

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
}

public class MoveSongData {
	public string Song { get; set; }
	public int Position { get; set; }
}
