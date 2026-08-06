using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class AboutMapper
    {
        public static About ToEntity(CreateAboutDto source)
        {
            return new About
            {
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static About ToEntity(UpdateAboutDto source)
        {
            return new About
            {
                AboutId = source.AboutId,
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static ResultAboutDto ToResult(About entity)
        {
            return new ResultAboutDto
            {
                AboutId = entity.AboutId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static GetAboutByIdDto? ToGetById(About? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetAboutByIdDto
            {
                AboutId = entity.AboutId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static List<ResultAboutDto>? ToResultList(IEnumerable<About>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
