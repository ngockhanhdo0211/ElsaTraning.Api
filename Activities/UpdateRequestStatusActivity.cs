using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using ElsaTraining.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ElsaTraining.Api.Activities;

public class UpdateRequestStatusActivity : CodeActivity
{
    [Input]
    public Input<Guid> RequestId { get; set; } = default!;

    [Input]
    public Input<string> Status { get; set; } = default!;

    [Input]
    public Input<string> CurrentStep { get; set; } = new("");

    [Input]
    public Input<string> CurrentApprover { get; set; } = new("");

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var dbContext = context.GetRequiredService<AppDbContext>();

        var requestId = RequestId.Get(context);
        var status = Status.Get(context);
        var currentStep = CurrentStep.Get(context);
        var currentApprover = CurrentApprover.Get(context);

        var request = await dbContext.ApprovalRequests
            .FirstOrDefaultAsync(x => x.Id == requestId, context.CancellationToken);

        if (request == null)
            return;

        request.Status = status;
        request.CurrentStep = string.IsNullOrWhiteSpace(currentStep) ? null : currentStep;
        request.CurrentApprover = string.IsNullOrWhiteSpace(currentApprover) ? null : currentApprover;

        if (status == "Submitted" && request.SubmittedAt == null)
            request.SubmittedAt = DateTime.UtcNow;

        if (status == "Approved" || status == "Rejected")
            request.DecisionAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}