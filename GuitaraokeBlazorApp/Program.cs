using GuitaraokeBlazorApp.Endpoints;
using GuitaraokeBlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
	.AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieJar, HttpCookieJar>();
builder.Services.AddScoped<IUserTracker, HttpCookieUserTracker>();

//Add Song data and services
var db = new SongDatabase(builder.Configuration["songs:filename"] ?? "songlist.txt");
if (builder.Configuration.GetValue<bool>("songs:sampledata")) {
	db.PopulateSampleData();
}
builder.Services.AddSingleton<ISongDatabase>(db);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	//app.UseWebAssemblyDebugging();
} else {
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapSongEndpoints();
app.MapBackstageEndpoints();

app.Run();

public partial class Program { } // so you can reference it from tests
