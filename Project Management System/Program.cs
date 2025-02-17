using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Management_System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SPMS_Context") ?? throw new InvalidOperationException("Connection string 'SPMS_ContextConnection' not found.");

builder.Services.AddDbContext<SPMS_Context>(options => options.UseSqlServer(connectionString));



builder.Services.AddDefaultIdentity<SPMS_User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<SPMS_Context>(); // Set up identity store

builder.Services.AddIdentityCore<SPMS_Student>().AddEntityFrameworkStores<SPMS_Context>();
builder.Services.AddIdentityCore<SPMS_Staff>().AddEntityFrameworkStores<SPMS_Context>(); // Set up seperate student and staff accounts

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SPMS_Context>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SPMS_Context>();
    context.Database.Migrate();
    var userMgr = services.GetRequiredService<UserManager<SPMS_User>>(); //Implementes HarrysPizzaUser so extra fields are implemented into identity.
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    SPMS_SeedUsers.Initialize(context, userMgr, roleMgr).Wait();
}

app.Run();
