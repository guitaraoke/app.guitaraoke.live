using GuitaraokeWebApp.Data;
using GuitaraokeWebApp.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GuitaraokeWebApp.Controllers;

public class RootController : Controller {
	private readonly ILogger<RootController> logger;
	private readonly ISongDatabase db;
	private readonly IUserTracker tracker;
	private readonly IHubContext<SongHub> hubContext;

	public RootController(ILogger<RootController> logger, ISongDatabase db, IUserTracker tracker, IHubContext<SongHub> hubContext) {
		this.logger = logger;
		this.db = db;
		this.tracker = tracker;
		this.hubContext = hubContext;
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
		if (user.SignUp(song, instruments)) {
			if (!String.IsNullOrEmpty(name)) user.Name = name;
			db.AddSongToQueue(song);
			var message = $"{song.Title} ({String.Join(", ", instruments.Select(i => i.ToString()))})";
			await hubContext.Clients.Group("backstage").SendAsync("signup", name, message);
		} else {
			db.PruneQueue();
		}
		return RedirectToAction("me");
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
			QueuedSongs = db.GetQueuedSongs(),
			StarredSongs = db.GetStarredSongs()
		};
		return View(songQueue);
	}

	public async Task<IActionResult> Me() {
		var user = tracker.GetUser();
		return View(user);
	}
}

