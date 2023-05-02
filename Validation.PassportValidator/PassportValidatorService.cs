using Microsoft.Extensions.Logging;
using RabbitMQ;
using RabbitMQ.Messages;

namespace Validation.PassportRabbitValidator
{
    public class PassportValidatorService
    {
        private RabbitMQRPCReciever<PassportMessageRequest, PassportMessageResponse> _reciever;

        public PassportValidatorService(ILogger logger)
        {
            _reciever = new RabbitMQRPCReciever<PassportMessageRequest, PassportMessageResponse>("PassportRequestQueue1", logger);
            _reciever.RPC = ValidateAsync;
        }

        private async Task<PassportMessageResponse> ValidateAsync(PassportMessageRequest message)
        {
            return Validate(message);
        }

        private PassportMessageResponse Validate(PassportMessageRequest message)
        {
            var response = new PassportMessageResponse();

            var data = message.PassportInfo;

            int rightLength = 10;
            response.SetResult(int.TryParse(data, out _) && data.Length == rightLength);
            return response;
        }
    }
}