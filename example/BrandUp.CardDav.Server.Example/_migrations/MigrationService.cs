namespace BrandUp.CardDav.Server.Example._migrations
{
    public class MigrationService : IHostedService
    {
        private readonly BrandUp.Extensions.Migrations.MigrationExecutor migrationExecutor;

        public MigrationService(BrandUp.Extensions.Migrations.MigrationExecutor migrationExecutor)
        {
            this.migrationExecutor = migrationExecutor ?? throw new ArgumentNullException(nameof(migrationExecutor));
        }

        #region IHostedService members

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            await migrationExecutor.UpAsync(cancellationToken);
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
