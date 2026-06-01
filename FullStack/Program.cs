using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Home/Index";
    });

// ✅ REGISTER DB CONTEXT HERE (BEFORE BUILD)
builder.Services.AddDbContext<FullStackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CivicIDDB")));

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<FullStackDbContext>();
        FullStack.Data.DbInitializer.Initialize(context);
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred seeding the DB: " + ex.Message);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Citizen}/{action=Index}/{id?}");

app.Run();