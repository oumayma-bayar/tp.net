using DashboardData.Components;
using DashboardData.Services;
using Microsoft.EntityFrameworkCore;
using DashboardData.Models;
using DashboardData.Data;
using Radzen;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<UserCounterService>();
builder.Services.AddTransient<UserCounterService>();
builder.Services.AddSingleton<UserCounterService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(o => o.DetailedErrors = true);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (await userManager.FindByEmailAsync("admin@data.com") == null)
    {
        var adminUser = new IdentityUser { UserName = "admin@data.com", Email = "admin@data.com" };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    if(!context.Sensors.Any())
    {
        Console.WriteLine("--Generation de donnees de test--");
        var labo = new Location{Name  ="Labo",Building = "Bat. A"};
        var usine = new Location{Name  ="Usine",Building = "Bat. B"};
        context.Locations.AddRange(labo, usine);
        
        var tagCritique = new Tag{Label = "Critique"};
        var tagMaintenance = new Tag{Label = "Maintenance"};
        context.Tags.AddRange(tagCritique, tagMaintenance);

        context.SaveChanges();

        var sonde1=new SensorData
        {
            Name ="Sonde_Alpha",Value =25.4,
            LocationId = labo.Id,
            Tags = new List<Tag>{tagCritique}
        };
          var sonde2=new SensorData
        {
            Name ="Sonde_Beta",Value =40.2,
            LocationId = usine.Id,
            Tags = new List<Tag>{tagCritique,tagMaintenance}
        };
        context.Sensors.AddRange(sonde1,sonde2);
        context.SaveChanges();


    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- AUTHENTICATION ENDPOINTS (Outside WebSocket) ---

app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email, 
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    
    if (result.Succeeded) return Results.Redirect("/dashboard");
    
    return Results.Redirect("/login?error=Invalid+credentials");
}).DisableAntiforgery(); 

app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();


app.Run(); // This line must always be the last in the file!
