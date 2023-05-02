using Grpc.Net.Client;
using Validation.NSPValidation;

namespace Validation.MainServer.Services
{
    public class NSPValidatorClient
    {
        private readonly ILogger<NSPValidatorClient> _logger;
        private readonly NSPValidator.NSPValidatorClient _client;
        public NSPValidatorClient(ILogger<NSPValidatorClient> logger = null)
        {
            _logger = logger;

            var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:7074",
                new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
            _client = new NSPValidator.NSPValidatorClient(channel);

        }

        public async Task<NSPResponse> ValidateAsync(NSPRequest request, CancellationToken token = default)
        {
            try
            {
                _logger?.LogInformation("validate async method run");
                var response = await _client.ValidateAsync(request, new Grpc.Core.CallOptions(cancellationToken: token));
                return response;
            }
            catch (Exception ex)
            {
                _logger?.LogError(nameof(ex));
                return null;
            }
            
        }
    }
}
