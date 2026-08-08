using DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos;
using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class SpyShipmentService : IShipmentService
{
    public int PublicTrackingLookupCount { get; private set; }

    public Task<List<ResultShipmentDto>> GetAllShipmentsAsync() =>
        Task.FromResult(new List<ResultShipmentDto>());

    public Task<long> GetTotalShipmentCountAsync() => Task.FromResult(0L);

    public Task<long> GetDeliveredShipmentCountAsync() => Task.FromResult(0L);

    public Task<int> GetDistinctDestinationCityCountAsync() => Task.FromResult(0);

    public Task<long> GetInDistributionShipmentCountAsync() => Task.FromResult(0L);

    public Task CreateShipmentAsync(CreateShipmentDto createShipmentDto) =>
        Task.CompletedTask;

    public Task UpdateShipmentAsync(UpdateShipmentDto updateShipmentDto) =>
        Task.CompletedTask;

    public Task<GetShipmentByIdDto> GetShipmentByIdAsync(string id) =>
        Task.FromResult(new GetShipmentByIdDto());

    public Task<GetShipmentByIdDto?> GetShipmentByTrackingNumberAsync(string trackingNumber) =>
        Task.FromResult<GetShipmentByIdDto?>(null);

    public Task<PublicTrackingResultDto?> GetPublicTrackingByTrackingNumberAsync(
        string trackingNumber)
    {
        PublicTrackingLookupCount++;
        return Task.FromResult<PublicTrackingResultDto?>(null);
    }

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task DeleteShipmentAsync(string id) => Task.CompletedTask;
}
