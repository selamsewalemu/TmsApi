using Microsoft.Extensions.DependencyInjection;

public class EnrollmentWorker(IServiceScopeFactory scopeFactory)
{
    public void ProcessBatch()
    {
        using var scope = scopeFactory.CreateScope();
        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        var existing = enrollmentService.GetAllAsync().GetAwaiter().GetResult();
        if (existing.Count == 0)
        {
            enrollmentService.EnrollAsync("S-001", "CS-101").GetAwaiter().GetResult();
        }
    }
}
