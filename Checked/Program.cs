using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Checked.Models.Models;
using Checked.Models.Email;
using Checked.Data;
using Checked.Servicos.Email;
using Checked.Servicos;
using Checked.Servicos.ControllerServices;
using Checked.Servicos.InviteService;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(config =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
})
    .AddViewLocalization();

builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion   = new ApiVersion(1, 0);
    options.ApiVersionReader = new MediaTypeApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
});

builder.Services.AddScoped<ActionsService>();
builder.Services.AddScoped<DashService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddScoped<PlansService>();
builder.Services.AddScoped<TaskService>();

builder.Services.AddWebOptimizer();

// CheckedContextConnectionDeveloper

var connectionString = builder.Configuration.GetConnectionString("CheckedContextConnection");
builder.Services.AddDbContext<CheckedDbContext>(options =>
{
    //options.UseSqlServer(connectionString), ServiceLifetime.Transient
    options.UseNpgsql(connectionString);
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<CheckedDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IMailService, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
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
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    //options.LoginPath = "/Identity/Account/Login";
    options.LoginPath = "/Home/Index";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromHours(5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


var supportedCultures = new[] { new CultureInfo("pt-BR") };
var requestLocalizationOptions = new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
}
.SetDefaultCulture("pt-BR");

app.UseRequestLocalization(requestLocalizationOptions);

app.UseWebOptimizer();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();

