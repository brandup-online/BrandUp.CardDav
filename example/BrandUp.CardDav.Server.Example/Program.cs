using BrandUp.CardDav.Server;
using BrandUp.CardDav.Server.Example._migrations;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Repositories;
using BrandUp.CardDav.Server.Extentions;

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable CA1416

builder.Logging
    .AddDebug()
    .AddConsole()
    .AddEventLog();

#pragma warning restore

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddControllers();

builder.Services.AddCradDavServer(o =>
{
    o.AuthPolicy = "Basic";
})
.AddRepositories<UserRepository, AddressBookRepository, ContactRepository>();
//.AddUsers<UserRepository>()
//.AddAddressBooks<AddressBookRepository>()
//.AddContacts<ContactRepository>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AddressBookRepository>();
builder.Services.AddScoped<ContactRepository>();

builder.Services.AddAuthentication().AddCardDavAuthentication();

builder.Services.AddMigrations(options =>
{
    options.AddAssembly(typeof(UserMigration).Assembly);
});
builder.Services.AddSingleton<BrandUp.Extensions.Migrations.IMigrationState, MigrationState>();
builder.Services.AddHostedService<MigrationService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMongoDbContext<AppDocumentContext>(builder.Configuration.GetSection("MongoDb"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }