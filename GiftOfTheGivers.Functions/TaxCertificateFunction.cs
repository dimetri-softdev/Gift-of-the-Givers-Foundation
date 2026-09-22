using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions
{
    public class TaxCertificateFunction
    {
        private readonly ILogger _logger;

        public TaxCertificateFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TaxCertificateFunction>();
        }

        [Function("GenerateTaxCertificate")]

        
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            // Gift of the Givers - Disaster Relief Automated Tax Certificate Generator
            _logger.LogInformation("Processing tax certificate generation...");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            string certNumber = $"TAX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var result = new
            {
                Message = "Tax Certificate Generated Successfully",
                CertificateNumber = certNumber,
                IssuedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await response.WriteStringAsync(System.Text.Json.JsonSerializer.Serialize(result));
            return response;
        }
    }
}
