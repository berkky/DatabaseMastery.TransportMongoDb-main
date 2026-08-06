using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class TestimonialMapper
    {
        public static Testimonial ToEntity(CreateTestimonialDto source)
        {
            return new Testimonial
            {
                NameSurname = source.NameSurname,
                Title = source.Title,
                ImageUrl = source.ImageUrl,
                ReviewDetail = source.ReviewDetail,
                ReviewScore = source.ReviewScore,
                Status = source.Status
            };
        }

        public static Testimonial ToEntity(UpdateTestimonialDto source)
        {
            return new Testimonial
            {
                TestimonialId = source.TestimonialId,
                NameSurname = source.NameSurname,
                Title = source.Title,
                ImageUrl = source.ImageUrl,
                ReviewDetail = source.ReviewDetail,
                ReviewScore = source.ReviewScore,
                Status = source.Status
            };
        }

        public static ResultTestimonialDto ToResult(Testimonial entity)
        {
            return new ResultTestimonialDto
            {
                TestimonialId = entity.TestimonialId,
                NameSurname = entity.NameSurname,
                Title = entity.Title,
                ImageUrl = entity.ImageUrl,
                ReviewDetail = entity.ReviewDetail,
                ReviewScore = entity.ReviewScore,
                Status = entity.Status
            };
        }

        public static GetTestimonialByIdDto? ToGetById(Testimonial? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetTestimonialByIdDto
            {
                TestimonialId = entity.TestimonialId,
                NameSurname = entity.NameSurname,
                Title = entity.Title,
                ImageUrl = entity.ImageUrl,
                ReviewDetail = entity.ReviewDetail,
                ReviewScore = entity.ReviewScore,
                Status = entity.Status
            };
        }

        public static List<ResultTestimonialDto>? ToResultList(IEnumerable<Testimonial>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
