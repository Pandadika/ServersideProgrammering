using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
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
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("WebApp");

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
    policy.RequireClaim("CprVerified");
  });
  options.AddPolicy("NotCprAuthenticatedUser", policy =>
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
    // Certficate authorzation er fjernet for den interne HttpClient. f�r hele tiden et SSL error n�r jeg pr�ver at k�re programmet i docker containere, 
    // Hvis man ikke k�rer i docker containere s� virker HttpClienten med certificates, jeg ved ikke hvorfor det ikke gider virke i docker.
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
  };
});


builder.Services.AddControllers();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

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

app.MapRazorComponents<WebApp.Components.App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
