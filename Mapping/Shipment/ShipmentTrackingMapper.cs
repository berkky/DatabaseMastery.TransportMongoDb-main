using DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping
{
    public static class ShipmentTrackingMapper
    {
        public static ShipmentTracking ToEntity(CreateShipmentTrackingDto source)
        {
            return new ShipmentTracking
            {
                EventDate = source.EventDate,
                Location = source.Location,
                Description = source.Description,
                TrackingStatus = source.TrackingStatus
            };
        }

        public static ResultShipmentTrackingDto ToResult(ShipmentTracking source)
        {
            return new ResultShipmentTrackingDto
            {
                EventDate = source.EventDate,
                Location = source.Location,
                Description = source.Description,
                TrackingStatus = source.TrackingStatus
            };
        }

        public static List<ResultShipmentTrackingDto>? ToResultList(
            IEnumerable<ShipmentTracking>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
