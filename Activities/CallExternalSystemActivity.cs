using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;

namespace ElsaTraning.Api.Activities;

public class CallExternalSystemActivity : CodeActivity
{
    public Input<string> Payload { get; set; } = default!;
    public Output<string> Result { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        // giả lập call API ngoài
        await Task.Delay(300, context.CancellationToken);

        var payload = Payload.Get(context);

        // giả lập response
        Result.Set(context, $"External system accepted payload length={payload?.Length ?? 0}");
    }
}