var songs = new List<Song> { new("Abba", "Waterloo") };
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ISongDatabase>(new SongDatabase(songs));
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.MapGet("/hello", () => "Hello World");
app.MapControllerRoute(
	name: "root",
	pattern: "{action=Index}/{id?}",
	defaults: new { controller = "Root" });
app.Run();

public partial class Program { }