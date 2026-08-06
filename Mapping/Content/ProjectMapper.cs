using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Mapping.Content
{
    public static class ProjectMapper
    {
        public static Project ToEntity(CreateProjectDto source)
        {
            return new Project
            {
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl,
                Status = source.Status
            };
        }

        public static Project ToEntity(UpdateProjectDto source)
        {
            return new Project
            {
                ProjectId = source.ProjectId,
                Title = source.Title,
                Description = source.Description,
                ImageUrl = source.ImageUrl,
                Status = source.Status
            };
        }

        public static ResultProjectDto ToResult(Project entity)
        {
            return new ResultProjectDto
            {
                ProjectId = entity.ProjectId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                Status = entity.Status
            };
        }

        public static GetProjectByIdDto? ToGetById(Project? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetProjectByIdDto
            {
                ProjectId = entity.ProjectId,
                Title = entity.Title,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                Status = entity.Status
            };
        }

        public static List<ResultProjectDto>? ToResultList(IEnumerable<Project>? source)
        {
            if (source is null)
            {
                return null;
            }

            return source.Select(ToResult).ToList();
        }
    }
}
