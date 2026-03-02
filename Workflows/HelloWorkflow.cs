using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Microsoft.AspNetCore.Http;

namespace ElsaTraning.Api.Workflows;

public class HelloWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var messageVar = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("hello"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                new SetVariable
                {
                    Variable = messageVar,
                    Value = new("Hello from Elsa V3 🚀")
                },

                new WriteHttpResponse
                {
                    Content = new(ctx => messageVar.Get(ctx)!)
                }
            }
        };
    }
}