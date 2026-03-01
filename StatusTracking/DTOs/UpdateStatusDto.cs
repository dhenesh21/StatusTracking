using StatusTracking.Models;

namespace StatusTracking.DTOs
{
    public class UpdateStatusDto
    {
        public IssueStatus Status { get; set; }
    }
}
