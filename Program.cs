using PLTreunion.Data;
using PLTreunion.Model;
using PLTreunion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PLTreunion.MyIdentity;

var builder = WebApplication.CreateBuilder(args);//initializes web app builder class
builder.Services.AddSingleton<IMyFakeData, MyFakeData>();

builder.Services.AddControllersWithViews();//adds services for controllers
builder.Services.AddDbContext<PLTDbContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("PLTConnectionString")));

builder.Services.AddDefaultIdentity<PLTUser>(
    options => {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
    }
    ).AddEntityFrameworkStores<PLTDbContext>();

var app = builder.Build();//add services to the container

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<PLTDbContext>();
//context.Database.EnsureDeleted();
context.Database.EnsureCreated();   
SeedMyDatabase.SeedDatabase(context);


if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    app.UseExceptionHandler("/Leader/Error");
    app.UseStatusCodePagesWithRedirects("/Leader/Error");
}

app.UseStaticFiles();
app.UseAuthentication();

app.UseRouting();//add route matching to the middleware pipeline
app.UseAuthorization();

app.MapControllerRoute(
    name: "SecondRoute",
    pattern: "Display/{id}",
    defaults: new {controller="Soldier", action="Show"}
    );



//app.MapDefaultControllerRoute();//use routing middleware to match url requests and map the requests to actions
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Leader}/{action=Index}/{id?}"
    );



//app.MapControllerRoute(
//    name: "AllRequests",
//    pattern: "{*Whatever}",
//    defaults: new { controller="Soldier", action="Index" }
//    );

app.Run();

