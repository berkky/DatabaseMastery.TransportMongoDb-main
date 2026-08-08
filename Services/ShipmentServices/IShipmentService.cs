using DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos;
using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;

namespace DatabaseMastery.TransportMongoDb.Services.ShipmentServices
{
    public interface IShipmentService
    {
        Task<List<ResultShipmentDto>> GetAllShipmentsAsync();

        Task<long> GetTotalShipmentCountAsync();

        Task<long> GetDeliveredShipmentCountAsync();

        Task<int> GetDistinctDestinationCityCountAsync();

        Task<long> GetInDistributionShipmentCountAsync();

        Task CreateShipmentAsync(CreateShipmentDto createShipmentDto);

        Task UpdateShipmentAsync(UpdateShipmentDto updateShipmentDto);

        Task<GetShipmentByIdDto> GetShipmentByIdAsync(string id);

        Task<GetShipmentByIdDto?> GetShipmentByTrackingNumberAsync(
            string trackingNumber);

        Task<PublicTrackingResultDto?> GetPublicTrackingByTrackingNumberAsync(
            string trackingNumber);

        Task DeleteShipmentAsync(string id);
    }
}
