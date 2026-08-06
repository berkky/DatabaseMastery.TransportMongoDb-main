using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class GetInTouchMapper
    {
        public static GetInTouchSection ToEntity(CreateGetInTouchDto source)
        {
            return new GetInTouchSection
            {
                BadgeTitle = source.BadgeTitle,
                MainTitle = source.MainTitle,
                Description = source.Description,
                Feature1Title = source.Feature1Title,
                Feature1Description = source.Feature1Description,
                Feature2Title = source.Feature2Title,
                Feature2Description = source.Feature2Description,
                ImageUrl = source.ImageUrl,
                Status = source.Status
            };
        }

        public static GetInTouchSection ToEntity(UpdateGetInTouchDto source)
        {
            return new GetInTouchSection
            {
                GetInTouchSectionId = source.GetInTouchSectionId,
                BadgeTitle = source.BadgeTitle,
                MainTitle = source.MainTitle,
                Description = source.Description,
                Feature1Title = source.Feature1Title,
                Feature1Description = source.Feature1Description,
                Feature2Title = source.Feature2Title,
                Feature2Description = source.Feature2Description,
                ImageUrl = source.ImageUrl,
                Status = source.Status
            };
        }

        public static ResultGetInTouchDto ToResult(GetInTouchSection entity)
        {
            return new ResultGetInTouchDto
            {
                GetInTouchSectionId = entity.GetInTouchSectionId,
                BadgeTitle = entity.BadgeTitle,
                MainTitle = entity.MainTitle,
                Description = entity.Description,
                Feature1Title = entity.Feature1Title,
                Feature1Description = entity.Feature1Description,
                Feature2Title = entity.Feature2Title,
                Feature2Description = entity.Feature2Description,
                ImageUrl = entity.ImageUrl,
                Status = entity.Status
            };
        }

        public static GetGetInTouchByIdDto? ToGetById(GetInTouchSection? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetGetInTouchByIdDto
            {
                GetInTouchSectionId = entity.GetInTouchSectionId,
                BadgeTitle = entity.BadgeTitle,
                MainTitle = entity.MainTitle,
                Description = entity.Description,
                Feature1Title = entity.Feature1Title,
                Feature1Description = entity.Feature1Description,
                Feature2Title = entity.Feature2Title,
                Feature2Description = entity.Feature2Description,
                ImageUrl = entity.ImageUrl,
                Status = entity.Status
            };
        }

        public static List<ResultGetInTouchDto>? ToResultList(IEnumerable<GetInTouchSection>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
