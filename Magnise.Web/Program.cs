using Contracts.Impl.Mappers;
using Contracts.Impl.Providers;
using Contracts.Impl.Services;
using Contracts.Providers;
using Contracts.Services;
using DataAccess;
using Magnise.Web.Handlers;
using Magnise.Web.Infrustructure;
using Magnise.Web.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection("Admin"));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(DefaulProfile));

builder.Services.AddDbContext<AssetsDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration["DbConnection"]));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAssetsProvider, AssetsProvider>();
builder.Services.AddScoped<IAssetsService, AssetsService>();

builder.Services.AddHostedService<BackgroundDbUpdateService>();

var app = builder.Build();

app.UseWebSockets();

app.UseStaticFiles();
app.UseSession();
app.MapControllers();

app.MapGet("/static", async context =>
{
    context.Response.Redirect("staticpage/static.html");
});

app.Map("/ws/{id}", WebSocketHandler.HandleClientConnectionAsync);

app.MapGet("/", () => "Hello, World!");

app.Run();
