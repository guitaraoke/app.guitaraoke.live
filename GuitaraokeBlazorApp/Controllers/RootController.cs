using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeBlazorApp.Controllers;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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

	//[HttpPost]
	//public async Task<IActionResult> Song(string slug, string name, Instrument[] instruments) {
	//	var song = db.FindSong(slug);
	//	if (song == default) return NotFound();
	//	var user = tracker.GetUser();
	//	if (user.SignUp(song, instruments)) {
	//		if (!String.IsNullOrEmpty(name)) user.Name = name;
	//		db.AddSongToQueue(song);
	//	} else {
	//		db.PruneQueue();
	//	}
	//	return RedirectToAction("me");
	//}

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
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

