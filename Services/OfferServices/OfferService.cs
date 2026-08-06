using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Mapping.Content;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.OfferServices
{
    public class OfferService : IOfferService
    {
        private readonly IMongoCollection<Offer> _offerCollection;

        public OfferService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _offerCollection = database.GetCollection<Offer>(
                databaseSettings.OfferCollectionName);
        }

        public async Task CreateOfferAsync(CreateOfferDto createOfferDto)
        {
            var value = OfferMapper.ToEntity(createOfferDto);
            await _offerCollection.InsertOneAsync(value);
        }

        public async Task DeleteOfferAsync(string id)
        {
            await _offerCollection.DeleteOneAsync(x => x.OfferId == id);
        }

        public async Task<List<ResultOfferDto>> GetAllOffersAsync()
        {
            var values = await _offerCollection
                .Find(x => true)
                .ToListAsync();

            return OfferMapper.ToResultList(values)!;
        }

        public async Task<GetOfferByIdDto> GetOfferByIdAsync(string id)
        {
            var value = await _offerCollection
                .Find(x => x.OfferId == id)
                .FirstOrDefaultAsync();

            return OfferMapper.ToGetById(value)!;
        }

        public async Task UpdateOfferAsync(UpdateOfferDto updateOfferDto)
        {
            var value = OfferMapper.ToEntity(updateOfferDto);

            await _offerCollection.FindOneAndReplaceAsync(
                x => x.OfferId == updateOfferDto.OfferId,
                value);
        }
    }
}
