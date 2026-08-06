using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class HowItWorksMapper
    {
        public static HowItWorks ToEntity(CreateHowItWorksDto source)
        {
            return new HowItWorks
            {
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static HowItWorks ToEntity(UpdateHowItWorksDto source)
        {
            return new HowItWorks
            {
                HowItWorksId = source.HowItWorksId,
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static ResultHowItWorksDto ToResult(HowItWorks entity)
        {
            return new ResultHowItWorksDto
            {
                HowItWorksId = entity.HowItWorksId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static GetHowItWorksByIdDto? ToGetById(HowItWorks? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetHowItWorksByIdDto
            {
                HowItWorksId = entity.HowItWorksId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static List<ResultHowItWorksDto>? ToResultList(IEnumerable<HowItWorks>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
