using GuitaraokeWebApp.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace GuitaraokeWebApp.Controllers;

public class BackstageController : Controller {
	private async Task TellClientsToRefreshQueue() {
		await hubContext.Clients.All.SendAsync("refresh-queue", "BackstageController", "");
	}

	private readonly ISongDatabase db;
	private readonly IHubContext<SongHub> hubContext;

	public BackstageController(ISongDatabase db, IHubContext<SongHub> hubContext) {
		this.db = db;
		this.hubContext = hubContext;
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
		await TellClientsToRefreshQueue();	
		return Json(true);
	}

	[HttpPost]
	public async Task<IActionResult> Status(string id, [FromBody] bool played) {
		var song = db.FindSong(id);
		if (song == default) return NotFound();
		song.PlayedAt = (played ? DateTime.UtcNow : null);
		if (!song.Played) return Json(played);
		db.RemoveSongFromQueue(song);
		await TellClientsToRefreshQueue();
		return Json(played);
	}

	public async Task<IActionResult> RemoveUser(Guid id) {
		db.RemoveUser(id);
		await TellClientsToRefreshQueue();
		return Redirect("/backstage");
	}
}

public class MoveSongData {
	public string Song { get; set; }
	public int Position { get; set; }
}
