using GuitaraokeBlazorApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddServerComponents();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieJar, HttpCookieJar>();
builder.Services.AddScoped<IUserTracker, HttpCookieUserTracker>();

//Add Song data and services
var songs = File.ReadAllLines("songlist.txt")
	.Select(line => line.Split(" - "))
	.Select(tokens => new Song(tokens[0], tokens[1]));
var db = new SongDatabase(songs);
#if DEBUG
db.PopulateSampleData();
#endif
builder.Services.AddSingleton<ISongDatabase>(db);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>();

app.MapControllers();

app.Run();

public partial class Program { } // so you can reference it from tests
