using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using ElsaTraining.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ElsaTraining.Api.Activities;

public class EscalateIfPendingActivity : CodeActivity
{
    [Input]
    public Input<Guid> RequestId { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var dbContext = context.GetRequiredService<AppDbContext>();
        var requestId = RequestId.Get(context);

        var request = await dbContext.ApprovalRequests
            .FirstOrDefaultAsync(x => x.Id == requestId, context.CancellationToken);

        if (request == null)
            return;

        // Chỉ escalate nếu vẫn đang pending
        if (request.Status == "PendingManagerApproval" || request.Status == "PendingFinanceApproval")
        {
            request.Status = "Escalated";
            request.CurrentStep = "Escalation";
            request.CurrentApprover = "SeniorManager";
            request.EscalatedAt = DateTime.UtcNow;
            request.EscalationNote = "Request quá hạn xử lý, hệ thống tự động escalate.";

            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}