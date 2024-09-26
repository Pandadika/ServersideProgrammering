using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Services;
using WebApp.Components.Account;
using WebApp.Data;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddSingleton<UserHandler, UserHandler>();
builder.Services.AddSingleton<IHashingService, HashingService>();

builder.Services.AddAuthentication(options =>
    {
      options.DefaultScheme = IdentityConstants.ApplicationScheme;
      options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AuthenticatedUser", policy =>
  {
    policy.RequireAuthenticatedUser();
  });
  options.AddPolicy("Admin", policy =>
  {
    policy.RequireRole("Admin");
  });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


var apiBaseAddress = builder.Configuration["ApiBaseAddress"];
if (string.IsNullOrEmpty(apiBaseAddress))
{
  throw new InvalidOperationException("Configuration value for 'ApiBaseAddress' is missing.");
}

builder.Services.AddHttpClient("DefaultClient", client =>
{
  client.BaseAddress = new Uri(apiBaseAddress);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
  return new HttpClientHandler
  {
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
  };
}); ;


builder.Services.AddControllers();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

//builder.WebHost.UseKestrel((context, serveroptions) =>
//{
//  serveroptions.Configure(context.Configuration.GetSection("kestrel"))
//  .Endpoint("https", listenoptions =>
//  {
//    listenoptions.HttpsOptions.SslProtocols = SslProtocols.Tls12;
//    listenoptions.HttpsOptions.ServerCertificate = new X509Certificate2(
//                Path.Combine("/https/cert.pfx"),
//                "Passw0rd");
//  });
//});

//string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//userFolder = Path.Combine(userFolder, ".aspnet");
//userFolder = Path.Combine(userFolder, "https");
//userFolder = Path.Combine(userFolder, "cert.pfx");
//builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Path").Value = userFolder;

//string kestrelPassword = builder.Configuration.GetValue<string>("KestrelPassword");
//builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Password").Value = kestrelPassword;

//builder.Services.AddDataProtection()
//  .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"))
//  .SetApplicationName("ServerProgApp");


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

//app.MapControllers();
app.MapRazorComponents<WebApp.Components.App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
