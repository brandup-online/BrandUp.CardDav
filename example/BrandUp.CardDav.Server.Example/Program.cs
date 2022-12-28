using BrandUp.CardDav.Server;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCradDavServer()
                .AddRepositories<UserRepository, AddressBookRepository, ContactRepository>();
//.AddUsers<UserRepository>()
//.AddAddressBooks<AddressBookRepository>()
//.AddContacts<ContactRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongoDbContext<AppDocumentContext>(builder.Configuration.GetSection("MongoDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
app.MapControllers();

app.Run();

public partial class Program { }