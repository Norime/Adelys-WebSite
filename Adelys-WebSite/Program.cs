using Adelys_WebSite.BL;
using Adelys_WebSite.DAL;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL;
using Adelys_WebSite.SAL.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//BDD
var connectionString = @"Data Source=(local);Initial Catalog=CodeFirstSQL;Integrated Security=True;TrustServerCertificate=True";
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<MySqlDbContext>(option => option.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IServiceMojangAccess, ServiceMojangAccess>();
builder.Services.AddTransient<IDataStorageBL, DataStorageBL>();
var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//await app.Services.GetService<IDataStorageBL>();
app.Run();
