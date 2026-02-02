using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddSingleton(new MyService());

        builder.Build().Run();
    }
}

public class MyService // : IMyService
{
    public MyService()
    {
        string temp = Environment.GetEnvironmentVariable("pause");
        Thread.Sleep(30000);
    }

    public async Task<string> GetStatus()
    {
        await Task.Delay(0);

        return "STATUS: MyService is running!";
    }

    public async Task DoWork()
    {
        if (System.Environment.GetEnvironmentVariable("pause").ToLower() == "true")
        {
            await Task.Delay(30000);
        }
    }
}

public interface IMyService
{
    public Task DoWork();
    public Task<string> GetStatus();
}