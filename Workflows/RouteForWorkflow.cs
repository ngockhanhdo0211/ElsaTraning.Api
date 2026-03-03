using System.Text.Json;
using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ElsaTraning.Api.Workflows;

public class RouteFormWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        // Workflow variables (state nằm trong instance)
        var amountVar = builder.WithVariable<decimal>();
        var deptVar = builder.WithVariable<string>();
        var routeVar = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                // 1) HTTP trigger
                new HttpEndpoint
                {
                    Path = new("route-form"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                // 2) Read input từ QueryString -> set variables
                new SetVariable
                {
                    Variable = deptVar,
                    Value = new(ctx =>
                    {
                        var http = ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!;
                        var dept = http.Request.Query["dept"].ToString();
                        return string.IsNullOrWhiteSpace(dept) ? "UNKNOWN" : dept;
                    })
                },
                new SetVariable
                {
                    Variable = amountVar,
                    Value = new(ctx =>
                    {
                        var http = ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!;
                        var raw = http.Request.Query["amount"].ToString();
                        return decimal.TryParse(raw, out var amount) ? amount : 0m;
                    })
                },

                // 3) If/Else route
                new If
                {
                    Condition = new(ctx => amountVar.Get(ctx) >= 50_000_000m),
                    Then = new SetVariable
                    {
                        Variable = routeVar,
                        Value = new("Director")
                    },
                    Else = new If
                    {
                        Condition = new(ctx => amountVar.Get(ctx) >= 10_000_000m),
                        Then = new SetVariable
                        {
                            Variable = routeVar,
                            Value = new("Manager")
                        },
                        Else = new SetVariable
                        {
                            Variable = routeVar,
                            Value = new("AutoApprove")
                        }
                    }
                },

                // 4) Return JSON response
                new WriteHttpResponse
                {
                    ContentType = new("application/json"),
                    Content = new(ctx =>
                    {
                        var payload = new
                        {
                            dept = deptVar.Get(ctx),
                            amount = amountVar.Get(ctx),
                            route = routeVar.Get(ctx)
                        };

                        return JsonSerializer.Serialize(payload);
                    })
                }
            }
        };
    }
}