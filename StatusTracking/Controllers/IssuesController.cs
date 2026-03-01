using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatusTracking.Data;
using StatusTracking.DTOs;
using StatusTracking.Models;

namespace StatusTracking.Controllers
{
    [ApiController]
    [Route("api/issues")]
    [Authorize]
    public class IssuesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IssuesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var issues = await _context.Issues
                .Include(i => i.AssignedToUser)
                .Select(i => new IssueDto
                {
                    IssueId = i.IssueId,
                    Title = i.Title,
                    Description = i.Description,
                    Priority = i.Priority.ToString(),
                    Status = i.Status.ToString(),
                    CreatedDate = i.CreatedDate,
                    UpdatedDate = i.UpdatedDate,
                    AssignedToUserId = i.AssignedToUserId,
                    AssignedToName = i.AssignedToUser != null
                        ? $"{i.AssignedToUser.FirstName} {i.AssignedToUser.LastName}"
                        : null
                })
                .ToListAsync();
            return Ok(issues);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var i = await _context.Issues
                .Include(i => i.AssignedToUser)
                .FirstOrDefaultAsync(i => i.IssueId == id);

            if (i == null)
                return NotFound(new { message = $"Issue with ID {id} not found." });

            return Ok(new IssueDto
            {
                IssueId = i.IssueId,
                Title = i.Title,
                Description = i.Description,
                Priority = i.Priority.ToString(),
                Status = i.Status.ToString(),
                CreatedDate = i.CreatedDate,
                UpdatedDate = i.UpdatedDate,
                AssignedToUserId = i.AssignedToUserId,
                AssignedToName = i.AssignedToUser != null
                    ? $"{i.AssignedToUser.FirstName} {i.AssignedToUser.LastName}"
                    : null
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateIssueDto dto)
        {
            if (dto.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);
                if (!userExists)
                    return BadRequest(new { message = $"User with ID {dto.AssignedToUserId} does not exist." });
            }

            var issue = new Issue
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                AssignedToUserId = dto.AssignedToUserId
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = issue.IssueId }, issue);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateIssueDto dto)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
                return NotFound(new { message = $"Issue with ID {id} not found." });

            if (dto.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);
                if (!userExists)
                    return BadRequest(new { message = $"User with ID {dto.AssignedToUserId} does not exist." });
            }

            issue.Title = dto.Title;
            issue.Description = dto.Description;
            issue.Priority = dto.Priority;
            issue.AssignedToUserId = dto.AssignedToUserId;
            issue.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(issue);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
                return NotFound(new { message = $"Issue with ID {id} not found." });

            issue.Status = dto.Status;
            issue.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Status updated.", status = issue.Status.ToString() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
                return NotFound(new { message = $"Issue with ID {id} not found." });

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Issue deleted successfully." });
        }
    }
}