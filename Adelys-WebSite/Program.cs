using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL;
using Adelys_WebSite.DAL.Repositories;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.SAL.Minecraft;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
// Réglages propres à la machine, ignorés par git : coordonnées des mentions légales
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

//BDD
var mySqlVersion = new MySqlServerVersion(new Version(8, 4, 0));
var connectionString = builder.Configuration.GetConnectionString("Luckperms") ?? string.Empty;
builder.Services.AddDbContext<MySqlDbContext>(option => option.UseMySql(connectionString, mySqlVersion));
var siteConnectionString = builder.Configuration.GetConnectionString("Site") ?? string.Empty;
builder.Services.AddDbContext<SiteDbContext>(option => option.UseMySql(siteConnectionString, mySqlVersion,
    mySql => mySql.MigrationsHistoryTable(SiteDbContext.MigrationsHistoryTable)));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<LegalInfo>(builder.Configuration.GetSection("Legal"));

//BL
builder.Services.AddTransient<IPlayerBL, PlayerBL>(); 
builder.Services.AddTransient<IUserPermissionBL, UserPermissionBL>();
builder.Services.AddScoped<ISiteUserBL, SiteUserBL>();
builder.Services.AddScoped<IGazetteBL, GazetteBL>();
builder.Services.AddScoped<IShopBL, ShopBL>();
builder.Services.AddScoped<IRulesBL, RulesBL>();
builder.Services.AddScoped<IEventBL, EventBL>();
builder.Services.AddScoped<IHomeSlideBL, HomeSlideBL>();
builder.Services.AddScoped<IHomeHeroBL, HomeHeroBL>();
builder.Services.AddSingleton<IMarkdownBL, MarkdownBL>();
builder.Services.AddScoped<IPasswordHasher<SiteUser>, PasswordHasher<SiteUser>>();

//DAL
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISiteUserRepository, SiteUserRepository>();
builder.Services.AddScoped<IGazetteRepository, GazetteRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IRulesRepository, RulesRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IHomeSlideRepository, HomeSlideRepository>();
builder.Services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();

//SAL
builder.Services.AddScoped<IServiceMojangAccess, ServiceMojangAccess>(); 
builder.Services.AddScoped<ICrafatarAccess, CrafatarAccess>();
builder.Services.AddSingleton<IGazetteFileStorage, GazetteFileStorage>();
builder.Services.AddSingleton<IContentImageStorage, ContentImageStorage>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<MinecraftItemCatalog>();
builder.Services.AddSingleton<IMinecraftItemCatalog>(services => services.GetRequiredService<MinecraftItemCatalog>());
builder.Services.AddHostedService<MinecraftCatalogUpdater>();

// Clés de chiffrement des cookies gardées hors du conteneur : sans elles, chaque redémarrage déconnecte tout le monde
string keysPath = Path.GetFullPath(builder.Configuration["DataProtection:KeysPath"] ?? "App_Data/Keys", builder.Environment.ContentRootPath);
builder.Services.AddDataProtection()
    .SetApplicationName("Adelys-WebSite")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

// Derrière le tunnel Cloudflare : le site reçoit du HTTP, l'en-tête X-Forwarded-Proto dit que le visiteur est en HTTPS
bool behindProxy = builder.Configuration.GetValue<bool>("ReverseProxy:Enabled");
if (behindProxy)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Le port du site n'est joignable que par cloudflared, sur le réseau Docker
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

//Authentification
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Adelys.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnValidatePrincipal = async context =>
        {
            ISiteUserBL siteUserBL = context.HttpContext.RequestServices.GetRequiredService<ISiteUserBL>();
            if (context.Principal == null || !siteUserBL.IsSessionValid(context.Principal))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };
    });

var app = builder.Build();

if (!string.IsNullOrEmpty(siteConnectionString))
{
    try
    {
        using IServiceScope scope = app.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<SiteDbContext>().Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Impossible de préparer la base du site, la connexion ne fonctionnera pas");
    }
}

// dotnet run -- create-admin <identifiant> : crée le premier administrateur, ou lui redonne l'accès
if (args.Length == 2 && args[0] == "create-admin")
{
    if (string.IsNullOrEmpty(siteConnectionString))
    {
        Console.Error.WriteLine("La chaîne de connexion ConnectionStrings:Site n'est pas configurée");
        return 1;
    }

    using IServiceScope scope = app.Services.CreateScope();
    ISiteUserBL siteUserBL = scope.ServiceProvider.GetRequiredService<ISiteUserBL>();
    SiteUser? admin = siteUserBL.GetUser(args[1]);
    string password;
    if (admin == null)
    {
        password = siteUserBL.CreateUser(args[1], SiteRoles.Admin);
    }
    else
    {
        siteUserBL.PromoteToAdmin(admin);
        password = siteUserBL.ResetPassword(admin);
    }
    Console.WriteLine($"Administrateur : {args[1]}");
    Console.WriteLine($"Mot de passe temporaire : {password}");
    return 0;
}

// Configure the HTTP request pipeline.
if (behindProxy)
{
    app.UseForwardedHeaders();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// Icônes des items, rangées par version : une adresse ne change jamais de contenu
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Services.GetRequiredService<MinecraftItemCatalog>().RootPath),
    RequestPath = MinecraftItemCatalog.IconsRequestPath,
    OnPrepareResponse = context => context.Context.Response.Headers.CacheControl = "public, max-age=2592000, immutable"
});

app.UseRouting();

app.UseAuthentication();

// Un mot de passe temporaire doit être changé avant de faire quoi que ce soit d'autre
app.Use(async (context, next) =>
{
    if (context.User.HasClaim(c => c.Type == SiteUserBL.MustChangePasswordClaim) && !context.Request.Path.StartsWithSegments("/Account"))
    {
        context.Response.Redirect("/Account/ChangePassword");
        return;
    }
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
return 0;
