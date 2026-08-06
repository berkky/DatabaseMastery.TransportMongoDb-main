using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class BrandMapper
    {
        public static Brand ToEntity(CreateBrandDto source)
        {
            return new Brand
            {
                BrandName = source.BrandName,
                ImageUrl = source.ImageUrl,
                IsStatus = source.IsStatus
            };
        }

        public static Brand ToEntity(UpdateBrandDto source)
        {
            return new Brand
            {
                BrandId = source.BrandId,
                BrandName = source.BrandName,
                ImageUrl = source.ImageUrl,
                IsStatus = source.IsStatus
            };
        }

        public static ResultBrandDto ToResult(Brand entity)
        {
            return new ResultBrandDto
            {
                BrandId = entity.BrandId,
                BrandName = entity.BrandName,
                ImageUrl = entity.ImageUrl,
                IsStatus = entity.IsStatus
            };
        }

        public static GetBrandIdDto? ToGetById(Brand? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetBrandIdDto
            {
                BrandId = entity.BrandId,
                BrandName = entity.BrandName,
                ImageUrl = entity.ImageUrl,
                IsStatus = entity.IsStatus
            };
        }

        public static List<ResultBrandDto>? ToResultList(IEnumerable<Brand>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
