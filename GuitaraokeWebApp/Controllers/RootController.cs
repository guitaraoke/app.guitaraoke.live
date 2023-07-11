using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeWebApp.Controllers;

public class RootController : Controller {
	private readonly ILogger<RootController> logger;

	public RootController(ILogger<RootController> logger) {
		this.logger = logger;
	}

	public IActionResult Index() => View();
}