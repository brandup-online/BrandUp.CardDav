using BrandUp.Extensions.Migrations;

namespace BrandUp.CardDav.Server.Controllers.Tests._migration
{
    public class TestMigrationState : IMigrationState
    {
        public TestMigrationState() { }
        public Task<bool> IsAppliedAsync(IMigrationDefinition migrationDefinition, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task SetDownAsync(IMigrationDefinition migrationDefinition, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SetUpAsync(IMigrationDefinition migrationDefinition, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
