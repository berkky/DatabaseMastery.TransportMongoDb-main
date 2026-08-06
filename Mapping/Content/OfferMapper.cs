using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class OfferMapper
    {
        public static Offer ToEntity(CreateOfferDto source)
        {
            return new Offer
            {
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl,
                IsStatus = source.IsStatus
            };
        }

        public static Offer ToEntity(UpdateOfferDto source)
        {
            return new Offer
            {
                OfferId = source.OfferId,
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl,
                IsStatus = source.IsStatus
            };
        }

        public static ResultOfferDto ToResult(Offer entity)
        {
            return new ResultOfferDto
            {
                OfferId = entity.OfferId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                IsStatus = entity.IsStatus
            };
        }

        public static GetOfferByIdDto? ToGetById(Offer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetOfferByIdDto
            {
                OfferId = entity.OfferId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                IsStatus = entity.IsStatus
            };
        }

        public static List<ResultOfferDto>? ToResultList(IEnumerable<Offer>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
