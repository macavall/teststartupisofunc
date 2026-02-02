using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace proj1;

public class http1
{
    private readonly ILogger<http1> _logger;
    private readonly IMyService _myService;

    public http1(ILogger<http1> logger, IMyService myService)
    {
        _logger = logger;
        _myService = myService;
    }

    [Function("http1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var temp = await _myService.GetStatus();

        _logger.LogInformation(temp);

        await _myService.DoWork();

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
