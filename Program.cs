using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddElsa(elsa =>
{
    // Management layer (Definitions)
    elsa.UseWorkflowManagement(management =>
        management.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));

    // Runtime layer (Instances, bookmarks, execution logs...)
    elsa.UseWorkflowRuntime(runtime =>
        runtime.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));
    elsa.UseHttp();
    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElsaTraining.Api v1");
});
app.UseWorkflows();
app.MapGet("/", () => "Elsa V3 Running 🚀");

app.Run();