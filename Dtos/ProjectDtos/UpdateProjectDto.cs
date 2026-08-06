namespace DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos
{
    public class UpdateProjectDto
    {
        public string ProjectId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
