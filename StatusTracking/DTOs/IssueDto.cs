namespace StatusTracking.DTOs
{
    public class IssueDto
    {
        public int IssueId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
    }
}
