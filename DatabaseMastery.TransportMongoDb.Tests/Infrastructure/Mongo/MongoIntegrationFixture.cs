using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Mongo;

public sealed class MongoIntegrationFixture : IAsyncLifetime
{
    private readonly string _ownedDatabaseName;
    private readonly string _connectionString;
    private MongoClient? _client;

    public MongoIntegrationFixture()
    {
        _connectionString = MongoTestSettings.DefaultConnectionString;
        _ownedDatabaseName = MongoTestSettings.GenerateOwnedDatabaseName();
    }

    public string OwnedDatabaseName => _ownedDatabaseName;

    public ShipmentService ShipmentService { get; private set; } = null!;

    public IMongoCollection<Shipment> Shipments =>
        _client!.GetDatabase(_ownedDatabaseName)
            .GetCollection<Shipment>(MongoTestSettings.ShipmentCollectionName);

    public async Task InitializeAsync()
    {
        MongoTestSettings.ValidateOwnedDatabaseName(_ownedDatabaseName, _ownedDatabaseName);
        MongoTestSettings.ValidateLocalConnectionString(_connectionString);

        _client = new MongoClient(_connectionString);

        await _client.GetDatabase("admin").RunCommandAsync<BsonDocument>(
            new BsonDocument("ping", 1));

        var settings = MongoTestSettings.CreateShipmentServiceSettings(_ownedDatabaseName);
        ShipmentService = new ShipmentService(settings);
    }

    public async Task DisposeAsync()
    {
        if (_client is null)
        {
            return;
        }

        MongoTestSettings.ValidateDropTarget(
            _ownedDatabaseName,
            _ownedDatabaseName,
            _connectionString);

        await _client.DropDatabaseAsync(_ownedDatabaseName);
        _client = null;
    }
}
