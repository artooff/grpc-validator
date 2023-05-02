using Microsoft.Extensions.Logging;
using RabbitMQ;
using RabbitMQ.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Validation.PhoneValidator
{
    public class PhoneValidatorService
    {
        private RabbitMQRPCReciever<PhoneMessageRequest, PhoneMessageResponse> _reciever;

        public PhoneValidatorService(ILogger logger)
        {
            _reciever = new RabbitMQRPCReciever<PhoneMessageRequest, PhoneMessageResponse>("PhoneRequestQueue", logger);
            _reciever.RPC = ValidateAsync;
        }

        private async Task<PhoneMessageResponse> ValidateAsync(PhoneMessageRequest message)
        {
            return Validate(message);
        }

        private PhoneMessageResponse Validate(PhoneMessageRequest message)
        {
            var response = new PhoneMessageResponse();
            var data = message.PhoneNumber;
            string pattern = @"^((8|\+7)[\-]?)?(\(?\d{3}\)?[\-]?)?[\d\-]{7,10}$";
            response.SetResult(Regex.IsMatch(data, pattern));
            return response;
        }


    }
}
