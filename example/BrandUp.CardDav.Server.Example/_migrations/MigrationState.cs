using BrandUp.Extensions.Migrations;

namespace BrandUp.CardDav.Server.Example._migrations
{
    public class MigrationState : IMigrationState
    {
        public MigrationState() { }
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
