namespace ElsaTraining.Api.Data.Entities;

public class ApprovalRequest
{
    public Guid Id { get; set; }

    public string RequesterName { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Department { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = "Draft";
    public string? CurrentStep { get; set; }
    public string? CurrentApprover { get; set; }

    public string? Decision { get; set; }
    public string? DecisionBy { get; set; }
    public string? RejectedReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public string? EscalationReason { get; set; }
    public string? EscalationNote { get; set; }
}