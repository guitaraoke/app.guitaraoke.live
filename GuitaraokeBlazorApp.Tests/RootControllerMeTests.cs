namespace GuitaraokeBlazorApp.Tests;

public class RootControllerMeTests : RootControllerTestBase {
	[Fact]
	public async Task Root_Me_Returns_View_With_User() {
		var c = MakeController();
		var result = await c.Me() as ViewResult;
		var model = result!.Model as User;
		model.ShouldNotBeNull();
	}
}
