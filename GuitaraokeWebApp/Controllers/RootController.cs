using GuitaraokeWebApp.Data;
using GuitaraokeWebApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeWebApp.Controllers;

public class RootController : Controller {
	private readonly ILogger<RootController> logger;
	private readonly ISongDatabase db;

	public RootController(ILogger<RootController> logger, ISongDatabase db) {
		this.logger = logger;
		this.db = db;
	}

	public async Task<IActionResult> Index() {
		var selection = db.ListSongs()
			.Select(song => new SongSelection(song));
		return View(selection);
	}
}

