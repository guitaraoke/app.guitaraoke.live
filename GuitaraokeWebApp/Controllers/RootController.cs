using GuitaraokeWebApp.Data;
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
		var user = tracker.GetUser();
		var stars = db.ListStarredSongs(user);
		var selection = db.ListSongs()
			.Select(song => new SongSelection(song) { IsStarred = stars.Contains(song) });
		return View(selection);
	}

	[HttpPost]
	public async Task<IActionResult> Star(string id) {
		var song = db.FindSong(id);
		if (song == default) return NotFound();
		var user = tracker.GetUser();
		return Json(db.ToggleStar(user, song));
	}

	[HttpPost]
	public async Task<IActionResult> Song(string slug, string name, Instrument[] instruments) {
		var song = db.FindSong(slug);
		if (song == default) return NotFound();
		var user = tracker.GetUser();
		db.SignUp(user, song, instruments);
		return Ok("yay!");
	}
	[HttpGet]
	public async Task<IActionResult> Song(string id) {
		var song = db.FindSong(id);
		if (song == default) return NotFound();
		var user = tracker.GetUser();
		var stars = db.ListStarredSongs(user);
		var model = new SongSelection(song) {
			User = user,
			IsStarred = stars.Contains(song)
		};
		return View(model);
	}

	public async Task<IActionResult> Queue() {
		var songQueue = new SongQueue {
			StarredSongs = db.ListStarredSongs()
				.ToDictionary(pair => pair.Key, pair => pair.Value.Count)
		};
		return View(songQueue);
	}

	public async Task<IActionResult> Me() {
		var user = tracker.GetUser();
		return View(user);
	}
}

