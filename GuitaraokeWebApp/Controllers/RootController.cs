using GuitaraokeWebApp.Data;
using GuitaraokeWebApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeWebApp.Controllers;

public class RootController : Controller {
	private readonly ILogger<RootController> logger;
	private readonly ISongDatabase db;
	private readonly IUserTracker tracker;

	public RootController(ILogger<RootController> logger, ISongDatabase db, IUserTracker tracker) {
		this.logger = logger;
		this.db = db;
		this.tracker = tracker;
	}

	public async Task<IActionResult> Index() {
		var userGuid = tracker.GetUserGuid();
		var stars = db.ListStarredSongs(userGuid);
		var selection = db.ListSongs()
			.Select(song => new SongSelection(song) { IsStarred = stars.Contains(song) });
		return View(selection);
	}

	public async Task<IActionResult> Star(string id) {
		var userGuid = tracker.GetUserGuid();
		var song = db.FindSong(id);
		return Json(db.ToggleStar(userGuid, song));
	}
}

