using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Models;

namespace ElsaTraning.Api.Activities;

public class ValidateFormActivity : CodeActivity
{
    public Input<string> Dept { get; set; } = default!;
    public Input<decimal> Amount { get; set; } = default!;

    public Output<bool> IsValid { get; set; } = default!;
    public Output<string> ErrorMessage { get; set; } = default!; // ✅ đổi sang string

    protected override void Execute(ActivityExecutionContext context)
    {
        var dept = Dept.Get(context);
        var amount = Amount.Get(context);

        if (string.IsNullOrWhiteSpace(dept))
        {
            IsValid.Set(context, false);
            ErrorMessage.Set(context, "Dept is required.");
            return;
        }

        if (amount <= 0)
        {
            IsValid.Set(context, false);
            ErrorMessage.Set(context, "Amount must be greater than 0.");
            return;
        }

        IsValid.Set(context, true);
        ErrorMessage.Set(context, ""); // ✅ không dùng null
    }
}