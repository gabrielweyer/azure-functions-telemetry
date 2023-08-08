using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.UserSecret;

public class UserSecretFunction
{
    private readonly SecretOptions _options;

    public UserSecretFunction(IOptions<SecretOptions> options)
    {
        _options = options.Value;
    }

    [Function(nameof(UserSecretFunction))]
    public async Task<HttpResponseData> RunGetSecretAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secret")]
        HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(_options, cancellationToken: request.FunctionContext.CancellationToken);
        return response;
    }
}
