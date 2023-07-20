using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GuitaraokeBlazorApp.Tests.WebTests;

public class WebTestBase : IClassFixture<WebApplicationFactory<Program>> {
	protected readonly WebApplicationFactory<Program> Factory = new();
	protected ISongDatabase Db => Factory.Services.GetService<ISongDatabase>()!;
	protected IBrowsingContext AngleSharp => BrowsingContext.New(Configuration.Default);
}
