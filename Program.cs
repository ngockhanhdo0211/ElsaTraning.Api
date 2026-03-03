using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;

//  để workflow đọc query/body qua HttpContext
builder.Services.AddHttpContextAccessor();

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management =>
        management.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));

    elsa.UseWorkflowRuntime(runtime =>
        runtime.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));

    elsa.UseHttp();

    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElsaTraining.Api v1");
    });
}

// ✅ host workflow endpoints (/workflows/*)
app.UseWorkflows();

app.MapGet("/", () => "Elsa V3 Running ");

app.Run();