using ElsaTraining.Api.Data;
using ElsaTraining.Api.Data.Entities;
using ElsaTraining.Api.Models;
using ElsaTraning.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElsaTraining.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ApprovalController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitApprovalRequestDto dto)
    {
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            RequesterName = dto.RequesterName,
            Title = dto.Title,
            Description = dto.Description,
            Department = dto.Department,
            Amount = dto.Amount,
            Status = "Submitted",
            CurrentStep = "ManagerApproval",
            CurrentApprover = "Manager",
            CreatedAt = DateTime.UtcNow,
            SubmittedAt = DateTime.UtcNow
        };

        _dbContext.ApprovalRequests.Add(request);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Submit thành công",
            requestId = request.Id,
            status = request.Status
        });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalDecisionDto dto)
    {
        var request = await _dbContext.ApprovalRequests.FindAsync(id);
        if (request == null)
            return NotFound("Không tìm thấy request.");

        request.Status = "Approved";
        request.Decision = "Approved";
        request.DecisionBy = dto.ManagerName;
        request.DecisionAt = DateTime.UtcNow;
        request.CurrentStep = null;
        request.CurrentApprover = null;

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Manager đã approve",
            requestId = request.Id,
            status = request.Status
        });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ApprovalDecisionDto dto)
    {
        var request = await _dbContext.ApprovalRequests.FindAsync(id);
        if (request == null)
            return NotFound("Không tìm thấy request.");

        request.Status = "Rejected";
        request.Decision = "Rejected";
        request.DecisionBy = dto.ManagerName;
        request.DecisionAt = DateTime.UtcNow;
        request.CurrentStep = null;
        request.CurrentApprover = null;
        request.RejectedReason = dto.Reason;

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Manager đã reject",
            requestId = request.Id,
            status = request.Status
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _dbContext.ApprovalRequests.FindAsync(id);
        if (request == null)
            return NotFound("Không tìm thấy request.");

        return Ok(request);
    }
}