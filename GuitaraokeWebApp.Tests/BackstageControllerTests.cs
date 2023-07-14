namespace GuitaraokeWebApp.Tests;

public class BackstageControllerTests {

	[Fact]
	public async Task BackstageIndexReturnsView() {
		var bc = new BackstageController();
		var result = await bc.Index() as ViewResult;
		result.ShouldNotBeNull();
	}
}
