var songs = File.ReadAllLines("songlist.txt")
	.Select(line => line.Split(" - "))
	.Select(tokens => new Song(tokens[0], tokens[1]));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserTracker, HttpCookieUserTracker>();
builder.Services.AddScoped<ICookieJar, HttpCookieJar>();
var db = new SongDatabase(songs);
#if DEBUG
// db.PopulateSampleData();
#endif
builder.Services.AddSingleton<ISongDatabase>(db);
builder.Services.Configure<RouteOptions>(config => {
	config.LowercaseQueryStrings = true;
	config.LowercaseUrls = true;
});
builder.Services.AddControllersWithViews();
#if DEBUG
builder.Services.AddSassCompiler();
#endif
var app = builder.Build();
app.UseStaticFiles();
app.MapGet("/hello", () => "Hello World");

app.MapControllerRoute(
	name: "root",
	pattern: "{action=Index}/{id?}",
	defaults: new { controller = "Root" });

app.MapControllerRoute(
	name: "controllers",
	pattern: "{controller}/{action=Index}/{id?}",
	defaults: new { controller = "Backstage" });

app.Run();

public partial class Program { }
