namespace DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos
{
    public class CreateProjectDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
