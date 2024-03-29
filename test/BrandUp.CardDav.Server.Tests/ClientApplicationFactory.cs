﻿using BrandUp.CardDav.Server.Controllers.Tests._migration;
using BrandUp.Extensions.Migrations;
using BrandUp.MongoDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server.Controllers
{
    public class ClientApplicationFactory : WebApplicationFactory<Program>
    {
        public ClientApplicationFactory() : base()
        {
            Server.AllowSynchronousIO = true;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddMigrations(options =>
                {
                    options.AddAssembly(typeof(TestUserMigration).Assembly);
                });
                services.AddSingleton<IMigrationState, TestMigrationState>();

                services.AddMongo2GoDbClientFactory();
            });
        }
    }
}
