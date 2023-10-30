using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyFunctionApp
{
    public static class HttpExample
    {
        [Function("HttpExample")]
        public static async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HttpExample");

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var response = req.CreateResponse();

            // Retrieve the secret from Azure Key Vault
            var keyVaultName = configuration["keyVaultName"];
            var secretName = configuration["secretName"];
            var KVUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(KVUri), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);

            // Use the retrieved secret
            var secretValue = secret.Value;

            logger.LogInformation($"Retrieved secret: {secretValue}");

            // Return the secret value in the HTTP response
            response.WriteString($"Retrieved secret: {secretValue}");

            return response;
        }
    }
}
