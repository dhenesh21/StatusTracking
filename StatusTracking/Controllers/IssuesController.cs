using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusTracking.Data;
using StatusTracking.DTOs;
using StatusTracking.Models;
using Microsoft.EntityFrameworkCore;

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
                .ToListAsync();

            return Ok(issues);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateIssueDto dto)
        {
            var issue = new Issue
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                AssignedToUserId = dto.AssignedToUserId
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateIssueDto dto)
        {
            var issue = await _context.Issues.FindAsync(id);

            if (issue == null)
                return NotFound();

            issue.Title = dto.Title;
            issue.Description = dto.Description;
            issue.Priority = dto.Priority;
            issue.AssignedToUserId = dto.AssignedToUserId;
            issue.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var issue = await _context.Issues.FindAsync(id);

            if (issue == null)
                return NotFound();

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully");
        }
    }
}
