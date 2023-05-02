using Grpc.Net.Client;
using Validation.PhoneNumberValidation;

namespace Validation.MainServer.Services
{
    public class PhoneNumberValidatorClient
    {
        private readonly ILogger<PhoneNumberValidatorClient> _logger;
        private readonly PhoneNumberValidator.PhoneNumberValidatorClient _client;
        public PhoneNumberValidatorClient(ILogger<PhoneNumberValidatorClient> logger = null)
        {
            _logger = logger;

            var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            //TODO: change address 
            var channel = GrpcChannel.ForAddress("https://localhost:7180",
                new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
            _client = new PhoneNumberValidator.PhoneNumberValidatorClient(channel);

        }

        public async Task<PhoneNumberResponse> ValidateAsync(PhoneNumberRequest request, CancellationToken token = default)
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
