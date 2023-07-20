namespace GuitaraokeBlazorApp.Tests.WebTests;

public class BackstageWebTests : WebTestBase {

	[Fact]
	public async Task Backstage_Index_Returns_OK() {
		var client = Factory.CreateClient();
		var response = await client.GetAsync("/backstage");
		response.IsSuccessStatusCode.ShouldBe(true);
	}
}
