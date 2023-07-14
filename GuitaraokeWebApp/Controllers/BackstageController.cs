using Microsoft.AspNetCore.Mvc;

namespace GuitaraokeWebApp.Controllers; 

public class BackstageController : Controller {

	public async Task<IActionResult> Index() {
		return View();
	}
}
