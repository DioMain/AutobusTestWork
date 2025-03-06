using autobusTestWork.Entity;
using autobusTestWork.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(new UrlHashService());

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

var nhibernate = new NHibernateManager(connectionString);

nhibernate.AddNHibernate(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
