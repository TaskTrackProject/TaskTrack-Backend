namespace TaskTrack.Repo.DTOs.Responses
{
    public class TagResponse
    {
        public int TagId { get; set; }

        public string TagName { get; set; } = string.Empty;

        public string? Color { get; set; }
    }
}
