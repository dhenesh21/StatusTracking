using StatusTracking.Models;

namespace StatusTracking.DTOs
{
    public class CreateIssueDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public int? AssignedToUserId { get; set; }
    }
}
