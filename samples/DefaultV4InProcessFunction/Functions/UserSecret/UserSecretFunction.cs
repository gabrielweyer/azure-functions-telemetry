using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.UserSecret;

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
        [SuppressMessage("Style", "IDE0060", Justification = "Can't use discard as it breaks the binding")]
        HttpRequest request)
    {
        return new OkObjectResult(_options);
    }
}
