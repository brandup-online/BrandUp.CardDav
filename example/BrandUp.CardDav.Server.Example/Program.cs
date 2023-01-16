using BrandUp.CardDav.Server;
using BrandUp.CardDav.Server.Example._migrations;
using BrandUp.CardDav.Server.Example.Authorization;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Repositories;

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

builder.Services.AddCradDavServer()
                .AddRepositories<UserRepository, AddressBookRepository, ContactRepository>();
//.AddUsers<UserRepository>()
//.AddAddressBooks<AddressBookRepository>()
//.AddContacts<ContactRepository>();

builder.Services.AddAuthentication().AddScheme<AuthenticationOptions, BasicAuthenticationHandler>("Basic", options =>
{

});

builder.Services.AddMigrations(options =>
{
    options.AddAssembly(typeof(UserMigration).Assembly);
});
builder.Services.AddSingleton<BrandUp.Extensions.Migrations.IMigrationState, MigrationState>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMongoDbContext<AppDocumentContext>(builder.Configuration.GetSection("MongoDb"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }