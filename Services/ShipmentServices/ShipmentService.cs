using DatabaseMastery.TransportMongoDb.Dtos.PublicTrackingDtos;
using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.ShipmentServices
{
    public class ShipmentService : IShipmentService
    {
        public const string TrackingNumberUniqueIndexName = "ux_shipments_tracking_number";

        private readonly IMongoCollection<Shipment> _shipmentCollection;

        public ShipmentService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _shipmentCollection = database.GetCollection<Shipment>(
                databaseSettings.ShipmentCollectionName);
        }

        public async Task CreateShipmentAsync(CreateShipmentDto createShipmentDto)
        {
            var value = ShipmentMapper.ToEntity(createShipmentDto);

            try
            {
                await _shipmentCollection.InsertOneAsync(value);
            }
            catch (Exception ex) when (IsDuplicateTrackingNumberException(ex))
            {
                throw new DuplicateTrackingNumberException();
            }
        }

        public async Task DeleteShipmentAsync(string id)
        {
            await _shipmentCollection.DeleteOneAsync(x => x.ShipmentId == id);
        }

        public async Task<List<ResultShipmentDto>> GetAllShipmentsAsync()
        {
            var values = await _shipmentCollection
                .Find(x => true)
                .ToListAsync();

            return ShipmentMapper.ToResultList(values)!;
        }

        public async Task<long> GetTotalShipmentCountAsync()
        {
            return await _shipmentCollection.CountDocumentsAsync(
                FilterDefinition<Shipment>.Empty);
        }

        public async Task<long> GetDeliveredShipmentCountAsync()
        {
            var filter = Builders<Shipment>.Filter.Eq(
                x => x.CurrentStatus,
                "Teslim Edildi");

            return await _shipmentCollection.CountDocumentsAsync(filter);
        }

        public async Task<int> GetDistinctDestinationCityCountAsync()
        {
            var cities = await _shipmentCollection.DistinctAsync<string>(
                "ArrivalCity",
                FilterDefinition<Shipment>.Empty);

            return await cities.ToListAsync().ContinueWith(t => t.Result.Count);
        }

        public async Task<long> GetInDistributionShipmentCountAsync()
        {
            var filter = Builders<Shipment>.Filter.Eq(
                x => x.CurrentStatus,
                "Dağıtımda");

            return await _shipmentCollection.CountDocumentsAsync(filter);
        }

        public async Task<GetShipmentByIdDto> GetShipmentByIdAsync(string id)
        {
            var value = await _shipmentCollection
                .Find(x => x.ShipmentId == id)
                .FirstOrDefaultAsync();

            return ShipmentMapper.ToGetById(value)!;
        }

        public async Task<GetShipmentByIdDto?> GetShipmentByTrackingNumberAsync(
            string trackingNumber)
        {
            var value = await _shipmentCollection
                .Find(x => x.TrackingNumber == trackingNumber)
                .FirstOrDefaultAsync();

            return value == null
                ? null
                : ShipmentMapper.ToGetById(value);
        }

        public async Task<PublicTrackingResultDto?> GetPublicTrackingByTrackingNumberAsync(
            string trackingNumber)
        {
            var filter = Builders<Shipment>.Filter.Eq(
                x => x.TrackingNumber,
                trackingNumber);

            var projection = Builders<Shipment>.Projection
                .Include(x => x.TrackingNumber)
                .Include(x => x.CurrentStatus)
                .Include(x => x.DepartureCity)
                .Include(x => x.ArrivalCity)
                .Include(x => x.CreatedDate)
                .Include("Trackings.EventDate")
                .Include("Trackings.TrackingStatus")
                .Exclude("_id");

            var projected = await _shipmentCollection
                .Find(filter)
                .Project<PublicShipmentProjection>(projection)
                .FirstOrDefaultAsync();

            if (projected is null)
            {
                return null;
            }

            return new PublicTrackingResultDto
            {
                TrackingNumber = projected.TrackingNumber,
                CurrentStatus = projected.CurrentStatus,
                DepartureCity = projected.DepartureCity,
                ArrivalCity = projected.ArrivalCity,
                CreatedDate = projected.CreatedDate,
                Events = projected.Trackings
                    .OrderByDescending(tracking => tracking.EventDate)
                    .Select(tracking => new PublicTrackingEventDto
                    {
                        EventDate = tracking.EventDate,
                        TrackingStatus = tracking.TrackingStatus
                    })
                    .ToList()
            };
        }

        public async Task UpdateShipmentAsync(UpdateShipmentDto updateShipmentDto)
        {
            var existingShipment = await _shipmentCollection
                .Find(x => x.ShipmentId == updateShipmentDto.ShipmentId)
                .FirstOrDefaultAsync();

            var value = ShipmentMapper.ToEntity(
                updateShipmentDto,
                existingShipment?.Trackings);

            try
            {
                await _shipmentCollection.FindOneAndReplaceAsync(
                    x => x.ShipmentId == updateShipmentDto.ShipmentId,
                    value);
            }
            catch (Exception ex) when (IsDuplicateTrackingNumberException(ex))
            {
                throw new DuplicateTrackingNumberException();
            }
        }

        public async Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
        {
            var indexKeys = Builders<Shipment>.IndexKeys
                .Ascending(x => x.TrackingNumber);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = TrackingNumberUniqueIndexName
            };

            await _shipmentCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<Shipment>(indexKeys, indexOptions),
                cancellationToken: cancellationToken);
        }

        private static bool IsDuplicateTrackingNumberException(Exception exception)
        {
            return exception switch
            {
                MongoWriteException writeException when
                    writeException.WriteError?.Category == ServerErrorCategory.DuplicateKey
                    => true,
                MongoBulkWriteException bulkWriteException =>
                    bulkWriteException.WriteErrors.Any(
                        writeError => writeError.Category == ServerErrorCategory.DuplicateKey),
                MongoCommandException commandException when commandException.Code == 11000
                    => true,
                _ => false
            };
        }

        private sealed class PublicShipmentProjection
        {
            public string TrackingNumber { get; set; } = string.Empty;

            public string CurrentStatus { get; set; } = string.Empty;

            public string DepartureCity { get; set; } = string.Empty;

            public string ArrivalCity { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; }

            public List<PublicTrackingEventProjection> Trackings { get; set; } = new();
        }

        private sealed class PublicTrackingEventProjection
        {
            public DateTime EventDate { get; set; }

            public string TrackingStatus { get; set; } = string.Empty;
        }
    }
}
