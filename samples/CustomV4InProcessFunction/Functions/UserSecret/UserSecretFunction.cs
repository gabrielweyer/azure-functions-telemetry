using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;

namespace CustomV4InProcessFunction.Functions.UserSecret;

public class UserSecretFunction
{
    private readonly SecretOptions _options;

    public UserSecretFunction(IOptions<SecretOptions> options)
    {
        _options = options.Value;
    }

    [FunctionName(nameof(UserSecretFunction))]
    public IActionResult RunGetSecret(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secret")]
        HttpRequest request)
    {
        return new OkObjectResult(_options);
    }
}

public class SecretOptions
{
    public string ReallySecretValue { get; set; }
}