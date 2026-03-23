using Elsa.Extensions;
using Elsa.Http;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using ElsaTraining.Api.Activities;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace ElsaTraning.Api.Workflows;

public class ApprovalWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var requestIdVar = builder.WithVariable<Guid>();
        var amountVar = builder.WithVariable<decimal>();
        var managerApprovedVar = builder.WithVariable<bool>();
        var financeApprovedVar = builder.WithVariable<bool>();
        var finalStatusVar = builder.WithVariable<string>();

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("approval-flow-v2/{requestId}/{amount}/{managerApproved}/{financeApproved}"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                new SetVariable
                {
                    Variable = requestIdVar,
                    Value = new(context =>
                    {
                        var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                        var path = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;
                        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                        return Guid.Parse(segments[^4]);
                    })
                },

                new SetVariable
                {
                    Variable = amountVar,
                    Value = new(context =>
                    {
                        var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                        var path = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;
                        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                        return decimal.Parse(segments[^3], CultureInfo.InvariantCulture);
                    })
                },

                new SetVariable
                {
                    Variable = managerApprovedVar,
                    Value = new(context =>
                    {
                        var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                        var path = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;
                        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                        return bool.Parse(segments[^2]);
                    })
                },

                new SetVariable
                {
                    Variable = financeApprovedVar,
                    Value = new(context =>
                    {
                        var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                        var path = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;
                        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                        return bool.Parse(segments[^1]);
                    })
                },

                // Luôn sang bước manager trước
                new UpdateRequestStatusActivity
                {
                    RequestId = new(context => requestIdVar.Get(context)),
                    Status = new("PendingManagerApproval"),
                    CurrentStep = new("ManagerApproval"),
                    CurrentApprover = new("Manager")
                },

                // Nếu manager reject -> dừng
                new If
                {
                    Condition = new(context => !managerApprovedVar.Get(context)),
                    Then = new Sequence
                    {
                        Activities =
                        {
                            new SetVariable
                            {
                                Variable = finalStatusVar,
                                Value = new("Rejected")
                            },
                            new UpdateRequestStatusActivity
                            {
                                RequestId = new(context => requestIdVar.Get(context)),
                                Status = new("Rejected"),
                                CurrentStep = new(""),
                                CurrentApprover = new("")
                            }
                        }
                    },
                    Else = new Sequence
                    {
                        Activities =
                        {
                            // Nếu amount nhỏ -> approved luôn
                            new If
                            {
                                Condition = new(context => amountVar.Get(context) < 5000000m),
                                Then = new Sequence
                                {
                                    Activities =
                                    {
                                        new SetVariable
                                        {
                                            Variable = finalStatusVar,
                                            Value = new("Approved")
                                        },
                                        new UpdateRequestStatusActivity
                                        {
                                            RequestId = new(context => requestIdVar.Get(context)),
                                            Status = new("Approved"),
                                            CurrentStep = new(""),
                                            CurrentApprover = new("")
                                        }
                                    }
                                },
                                Else = new Sequence
                                {
                                    Activities =
                                    {
                                        // Amount lớn -> qua finance
                                        new UpdateRequestStatusActivity
                                        {
                                            RequestId = new(context => requestIdVar.Get(context)),
                                            Status = new("PendingFinanceApproval"),
                                            CurrentStep = new("FinanceApproval"),
                                            CurrentApprover = new("Finance")
                                        },

                                        // Finance quyết định
                                        new If
                                        {
                                            Condition = new(context => financeApprovedVar.Get(context)),
                                            Then = new Sequence
                                            {
                                                Activities =
                                                {
                                                    new SetVariable
                                                    {
                                                        Variable = finalStatusVar,
                                                        Value = new("Approved")
                                                    },
                                                    new UpdateRequestStatusActivity
                                                    {
                                                        RequestId = new(context => requestIdVar.Get(context)),
                                                        Status = new("Approved"),
                                                        CurrentStep = new(""),
                                                        CurrentApprover = new("")
                                                    }
                                                }
                                            },
                                            Else = new Sequence
                                            {
                                                Activities =
                                                {
                                                    new SetVariable
                                                    {
                                                        Variable = finalStatusVar,
                                                        Value = new("Rejected")
                                                    },
                                                    new UpdateRequestStatusActivity
                                                    {
                                                        RequestId = new(context => requestIdVar.Get(context)),
                                                        Status = new("Rejected"),
                                                        CurrentStep = new(""),
                                                        CurrentApprover = new("")
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },

                new WriteHttpResponse
                {
                    Content = new(context =>
                    {
                        var requestId = requestIdVar.Get(context);
                        var amount = amountVar.Get(context);
                        var managerApproved = managerApprovedVar.Get(context);
                        var financeApproved = financeApprovedVar.Get(context);
                        var finalStatus = finalStatusVar.Get(context) ?? "Unknown";

                        return $"""
                        Approval workflow Day 7 completed.
                        RequestId: {requestId}
                        Amount: {amount}
                        ManagerApproved: {managerApproved}
                        FinanceApproved: {financeApproved}
                        FinalStatus: {finalStatus}
                        """;
                    })
                }
            }
        };
    }
}