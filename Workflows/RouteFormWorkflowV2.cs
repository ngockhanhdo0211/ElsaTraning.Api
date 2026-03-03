using System.Text.Json;
using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ElsaTraning.Api.Activities;

namespace ElsaTraning.Api.Workflows;

public class RouteFormWorkflowV2 : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var amountVar = builder.WithVariable<decimal>();
        var deptVar = builder.WithVariable<string>();
        var routeVar = builder.WithVariable<string>();

        var isValidVar = builder.WithVariable<bool>();
        var errorVar = builder.WithVariable<string?>();

        var externalResultVar = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("route-form-v2"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                // Read query
                new SetVariable
                {
                    Variable = deptVar,
                    Value = new(ctx =>
                    {
                        var http = ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!;
                        return http.Request.Query["dept"].ToString();
                    })
                },
                new SetVariable
                {
                    Variable = amountVar,
                    Value = new(ctx =>
                    {
                        var http = ctx.GetRequiredService<IHttpContextAccessor>().HttpContext!;
                        var raw = http.Request.Query["amount"].ToString();
                        return decimal.TryParse(raw, out var a) ? a : 0m;
                    })
                },

                // ✅ Custom Validate
                new ValidateFormActivity
                {
                    Dept = new(ctx => deptVar.Get(ctx) ?? ""),
                    Amount = new(ctx => amountVar.Get(ctx)),
                    IsValid = new(isValidVar),
                    ErrorMessage = new(errorVar)
                },

                // If invalid -> return 400
                new If
                {
                    Condition = new(ctx => isValidVar.Get(ctx) == false),
                    Then = new WriteHttpResponse
                    {
                        ContentType = new("application/json"),
                        Content = new(ctx => JsonSerializer.Serialize(new
                        {
                            ok = false,
                            error = errorVar.Get(ctx)
                        }))
                    },
                    Else = new Sequence
                    {
                        Activities =
                        {
                            // routing (giống Day 3)
                            new If
                            {
                                Condition = new(ctx => amountVar.Get(ctx) >= 50_000_000m),
                                Then = new SetVariable { Variable = routeVar, Value = new("Director") },
                                Else = new If
                                {
                                    Condition = new(ctx => amountVar.Get(ctx) >= 10_000_000m),
                                    Then = new SetVariable { Variable = routeVar, Value = new("Manager") },
                                    Else = new SetVariable { Variable = routeVar, Value = new("AutoApprove") }
                                }
                            },

                            // ✅ Custom Call External
                            new CallExternalSystemActivity
                            {
                                Payload = new(ctx => JsonSerializer.Serialize(new
                                {
                                    dept = deptVar.Get(ctx),
                                    amount = amountVar.Get(ctx),
                                    route = routeVar.Get(ctx)
                                })),
                                Result = new(externalResultVar)
                            },

                            // Return OK
                            new WriteHttpResponse
                            {
                                ContentType = new("application/json"),
                                Content = new(ctx => JsonSerializer.Serialize(new
                                {
                                    ok = true,
                                    dept = deptVar.Get(ctx),
                                    amount = amountVar.Get(ctx),
                                    route = routeVar.Get(ctx),
                                    external = externalResultVar.Get(ctx)
                                }))
                            }
                        }
                    }
                }
            }
        };
    }
}