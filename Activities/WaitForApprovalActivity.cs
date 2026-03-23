namespace ElsaTraining.Api.Activities;

public class WaitForApprovalActivity
{
    // Activity này sẽ là bước chờ manager approve/reject.
    // Ý tưởng:
    // 1. Workflow đi tới đây thì tạm dừng.
    // 2. Chờ tác động từ API approve/reject.
    // 3. Sau đó workflow chạy tiếp theo nhánh Approved hoặc Rejected.
}