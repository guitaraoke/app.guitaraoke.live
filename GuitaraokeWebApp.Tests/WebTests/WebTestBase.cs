namespace GuitaraokeWebApp.Tests.WebTests;

public class WebTestBase : IClassFixture<WebApplicationFactory<Program>> {
	protected readonly WebApplicationFactory<Program> Factory = new();
	protected ISongDatabase Db => Factory.Services.GetService<ISongDatabase>()!;
	protected IBrowsingContext AngleSharp => BrowsingContext.New(Configuration.Default);
}
