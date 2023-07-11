var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.MapGet("/hello", () => "Hello World");
app.MapControllerRoute(
	name: "root",
	pattern: "{action=Index}/{id?}", 
	defaults: new { controller = "Root" });
app.Run();


