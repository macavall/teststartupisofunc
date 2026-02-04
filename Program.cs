using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private static void Log(string message)
    {
        Console.WriteLine($"[{_stopwatch.ElapsedMilliseconds,6}ms] {message}");
    }

    private static void Main(string[] args)
    {
        Log("Program.Main started");

        var builder = FunctionsApplication.CreateBuilder(args);
        Log("FunctionsApplication.CreateBuilder completed");

        builder.ConfigureFunctionsWebApplication();
        Log("ConfigureFunctionsWebApplication completed");

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();
        Log("Application Insights configured");

        // ============================================================
        // DEMO: Register singletons that simulate slow initialization
        // ============================================================
        builder.Services.AddSingleton<ISlowService, SlowService>();
        builder.Services.AddSingleton<IAnotherSlowService, AnotherSlowService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        Log("Singleton services registered (NO instantiation yet)");

        // ============================================================
        // ANTI-PATTERN: Premature BuildServiceProvider() call
        // This builds a SEPARATE, throwaway DI container
        // ============================================================
        Log(">>> About to call BuildServiceProvider() - ANTI-PATTERN <<<");
        var prematureProvider = builder.Services.BuildServiceProvider();
        Log(">>> BuildServiceProvider() returned (container built, but singletons NOT instantiated yet)");

        // Now let's force resolution to show when instantiation happens
        Log(">>> Resolving ISlowService from premature provider...");
        var slow1 = prematureProvider.GetRequiredService<ISlowService>();
        Log($">>> ISlowService resolved: {slow1.Name}");

        Log(">>> Resolving IAnotherSlowService from premature provider...");
        var slow2 = prematureProvider.GetRequiredService<IAnotherSlowService>();
        Log($">>> IAnotherSlowService resolved: {slow2.Name}");

        Log(">>> Resolving IDatabaseService from premature provider...");
        var slow3 = prematureProvider.GetRequiredService<IDatabaseService>();
        Log($">>> IDatabaseService resolved: {slow3.Name}");

        // ============================================================
        // THE REAL BUILD - Creates ANOTHER container (wasted work above!)
        // ============================================================
        Log("=== Calling builder.Build() - This creates the REAL container ===");
        var host = builder.Build();
        Log("=== builder.Build() completed ===");

        // Singletons will be instantiated AGAIN in this new container on first resolve!
        Log("=== Calling host.Run() ===");
        host.Run();
    }
}

// ============================================================
// Simulated slow services to demonstrate instantiation timing
// ============================================================


