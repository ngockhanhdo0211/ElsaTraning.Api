namespace ElsaTraining.Api.Models;

public class SubmitApprovalRequestDto
{
    public string RequesterName { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Department { get; set; }
    public decimal Amount { get; set; }
}