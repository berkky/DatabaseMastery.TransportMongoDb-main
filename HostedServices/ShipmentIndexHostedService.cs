using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;

namespace DatabaseMastery.TransportMongoDb.HostedServices
{
    public sealed class ShipmentIndexHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ShipmentIndexHostedService> _logger;

        public ShipmentIndexHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<ShipmentIndexHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider
                .GetRequiredService<IShipmentService>();

            await shipmentService.EnsureIndexesAsync(cancellationToken);
            _logger.LogInformation("Shipment indexes ensured.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
