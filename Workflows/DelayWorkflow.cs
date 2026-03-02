using Elsa.Http;
using Elsa.Scheduling.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;
using Microsoft.AspNetCore.Http;

namespace ElsaTraning.Api.Workflows;

public class DelayWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var secondsVar = builder.WithVariable<int>();
        var resumedVar = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("delay"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                new SetVariable
                {
                    Variable = secondsVar,
                    Value = new(10)
                },

                new WriteHttpResponse
                {
                    Content = new(ctx => $"Workflow started. Will resume after {secondsVar.Get(ctx)} seconds.")
                },

                new Delay
                {
                    TimeSpan = new(ctx => TimeSpan.FromSeconds(secondsVar.Get(ctx)))
                },

                new SetVariable
                {
                    Variable = resumedVar,
                    Value = new("✅ Resumed after delay")
                },

                // ✅ Force đúng activity WriteLine + đúng Input<string>
                new Elsa.Workflows.Activities.WriteLine(
                    new Input<string>(ctx => resumedVar.Get(ctx)!)
                )
            }
        };
    }
}