using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class SlowService : ISlowService
{
    public string Name { get; }

    public SlowService()
    {
        Console.WriteLine($"[{Program_Timestamp()}] SlowService constructor STARTED - simulating 500ms delay...");
        Thread.Sleep(5000); // Simulate slow initialization (loading config, etc.)
        Name = $"SlowService-{Guid.NewGuid().ToString()[..8]}";
        Console.WriteLine($"[{Program_Timestamp()}] SlowService constructor COMPLETED: {Name}");
    }

    private static string Program_Timestamp() => $"{Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000),6}ms";
}

public class AnotherSlowService : IAnotherSlowService
{
    public string Name { get; }

    public AnotherSlowService()
    {
        Console.WriteLine($"[{Program_Timestamp()}] AnotherSlowService constructor STARTED - simulating 300ms delay...");
        Thread.Sleep(10000); // Simulate slow initialization
        Name = $"AnotherSlowService-{Guid.NewGuid().ToString()[..8]}";
        Console.WriteLine($"[{Program_Timestamp()}] AnotherSlowService constructor COMPLETED: {Name}");
    }

    private static string Program_Timestamp() => $"{Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000),6}ms";
}



public class DatabaseService : IDatabaseService
{
    public string Name { get; }

    public DatabaseService()
    {
        Console.WriteLine($"[{Program_Timestamp()}] DatabaseService constructor STARTED - simulating BLOCKING async call (800ms)...");
        
        // ANTI-PATTERN: Blocking on async - simulates BuildSqlConnectionStringAsync().GetAwaiter().GetResult()
        SimulateAsyncDatabaseConnection().GetAwaiter().GetResult();
        
        Name = $"DatabaseService-{Guid.NewGuid().ToString()[..8]}";
        Console.WriteLine($"[{Program_Timestamp()}] DatabaseService constructor COMPLETED: {Name}");
    }

    private static async Task SimulateAsyncDatabaseConnection()
    {
        // Simulates: Azure AD token acquisition, DNS lookup, connection establishment
        await Task.Delay(800);
    }

    private static string Program_Timestamp() => $"{Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000),6}ms";
}