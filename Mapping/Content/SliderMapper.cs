using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class SliderMapper
    {
        public static Slider ToEntity(CreateSliderDto source)
        {
            return new Slider
            {
                SliderTitle = source.SliderTitle,
                Subtitle = source.Subtitle,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static Slider ToEntity(UpdateSliderDto source)
        {
            return new Slider
            {
                SliderId = source.SliderId,
                SliderTitle = source.SliderTitle,
                Subtitle = source.Subtitle,
                Description = source.Description,
                ImageUrl = source.ImageUrl
            };
        }

        public static ResultSliderDto ToResult(Slider entity)
        {
            return new ResultSliderDto
            {
                SliderId = entity.SliderId,
                SliderTitle = entity.SliderTitle,
                Subtitle = entity.Subtitle,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static GetSliderByIdDto? ToGetById(Slider? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetSliderByIdDto
            {
                SliderId = entity.SliderId,
                SliderTitle = entity.SliderTitle,
                Subtitle = entity.Subtitle,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static List<ResultSliderDto>? ToResultList(IEnumerable<Slider>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
