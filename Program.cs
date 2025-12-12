using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using Microsoft.AspNetCore.Identity;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using LibraryManagement.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is missing.");
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is missing.");
        googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]
            ?? throw new InvalidOperationException("Microsoft ClientId is missing.");
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]
            ?? throw new InvalidOperationException("Microsoft ClientSecret is missing.");
        microsoftOptions.SignInScheme = IdentityConstants.ExternalScheme;
    });


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseSwaggerUi(options =>
    // {
    //     options.DocumentPath = "/openapi/v1.json";
    // });
}
else
{
    // Option 1:
    // redirection
    app.UseExceptionHandler("/Identity/Error");


    // Option 2:
    // Exception handler lambda
    // app.UseExceptionHandler(exceptionHandlerApp =>
    // {
    //     exceptionHandlerApp.Run(async context =>
    //     {
    //         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //         // using static System.Net.Mime.MediaTypeNames;
    //         context.Response.ContentType = "text/html";//Text.Plain;
    //         await context.Response.WriteAsync("An exception was thrown.");
    //         var exceptionHandlerPathFeature =
    //         context.Features.Get<IExceptionHandlerPathFeature>();
    //         if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
    //         {
    //             await context.Response.WriteAsync(" The file was not found.");
    //         }
    //         if (exceptionHandlerPathFeature?.Path == "/")
    //         {
    //             await context.Response.WriteAsync(" Page: Home.");
    //         }
    //     });
    // });

    app.UseHsts();
}

// Option 1:
// app.UseStatusCodePages();

// Option 2:
// app.UseStatusCodePages("text/html", "Status Code Page: {0}");

// Option 3:
// app.UseStatusCodePages(async statusCodeContext =>
// {
//     // using static System.Net.Mime.MediaTypeNames;
//     statusCodeContext.HttpContext.Response.ContentType = "text/html";
//     await statusCodeContext.HttpContext.Response.WriteAsync(
//     $"Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
// });

// Option 4:
// app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");

// Option 5:
// app.UseStatusCodePagesWithReExecute("/Identity/Error", "?StatusCode={0}");


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // add authentication middleware
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // Map Razor Pages routes

app.Run();
