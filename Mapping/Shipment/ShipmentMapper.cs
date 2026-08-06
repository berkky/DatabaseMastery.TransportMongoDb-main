using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping
{
    public static class ShipmentMapper
    {
        public static Shipment ToEntity(CreateShipmentDto source)
        {
            return new Shipment
            {
                TrackingNumber = source.TrackingNumber,
                SenderName = source.SenderName,
                SenderPhone = source.SenderPhone,
                ReceiverName = source.ReceiverName,
                ReceiverPhone = source.ReceiverPhone,
                DepartureCity = source.DepartureCity,
                DepartureDistrict = source.DepartureDistrict,
                ArrivalCity = source.ArrivalCity,
                ArrivalDistrict = source.ArrivalDistrict,
                Address = source.Address,
                CreatedDate = source.CreatedDate,
                CurrentStatus = source.CurrentStatus
            };
        }

        public static Shipment ToEntity(
            UpdateShipmentDto source,
            List<ShipmentTracking>? existingTrackings)
        {
            return new Shipment
            {
                ShipmentId = source.ShipmentId,
                TrackingNumber = source.TrackingNumber,
                SenderName = source.SenderName,
                SenderPhone = source.SenderPhone,
                ReceiverName = source.ReceiverName,
                ReceiverPhone = source.ReceiverPhone,
                DepartureCity = source.DepartureCity,
                DepartureDistrict = source.DepartureDistrict,
                ArrivalCity = source.ArrivalCity,
                ArrivalDistrict = source.ArrivalDistrict,
                Address = source.Address,
                CreatedDate = source.CreatedDate,
                CurrentStatus = source.CurrentStatus,
                Trackings = existingTrackings!
            };
        }

        public static ResultShipmentDto ToResult(Shipment entity)
        {
            return new ResultShipmentDto
            {
                ShipmentId = entity.ShipmentId,
                TrackingNumber = entity.TrackingNumber,
                SenderName = entity.SenderName,
                SenderPhone = entity.SenderPhone,
                ReceiverName = entity.ReceiverName,
                ReceiverPhone = entity.ReceiverPhone,
                DepartureCity = entity.DepartureCity,
                DepartureDistrict = entity.DepartureDistrict,
                ArrivalCity = entity.ArrivalCity,
                ArrivalDistrict = entity.ArrivalDistrict,
                Address = entity.Address,
                CreatedDate = entity.CreatedDate,
                CurrentStatus = entity.CurrentStatus
            };
        }

        public static GetShipmentByIdDto? ToGetById(Shipment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetShipmentByIdDto
            {
                ShipmentId = entity.ShipmentId,
                TrackingNumber = entity.TrackingNumber,
                SenderName = entity.SenderName,
                SenderPhone = entity.SenderPhone,
                ReceiverName = entity.ReceiverName,
                ReceiverPhone = entity.ReceiverPhone,
                DepartureCity = entity.DepartureCity,
                DepartureDistrict = entity.DepartureDistrict,
                ArrivalCity = entity.ArrivalCity,
                ArrivalDistrict = entity.ArrivalDistrict,
                Address = entity.Address,
                CreatedDate = entity.CreatedDate,
                CurrentStatus = entity.CurrentStatus,
                Trackings = entity.Trackings
            };
        }

        public static List<ResultShipmentDto>? ToResultList(IEnumerable<Shipment>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
