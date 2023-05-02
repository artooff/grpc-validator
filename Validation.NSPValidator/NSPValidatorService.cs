using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ;
using RabbitMQ.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Validation.NSPRabbitValidator
{
    public class NSPValidatorService
    {
        private RabbitMQRPCReciever<NSPMessageRequest, NSPMessageResponse> _receiver;

        public NSPValidatorService(ILogger logger)
        {
            _receiver = new("NSPRequestQueue", logger);
            _receiver.RPC = ValidateAsync;
        }

        private async Task<NSPMessageResponse> ValidateAsync(NSPMessageRequest message)
        {
            return Validate(message);
        }

        private NSPMessageResponse Validate(NSPMessageRequest message)
        {
            var response = new NSPMessageResponse();

            response.SurnameResult = ValidateNspPart(message.Surname);
            response.NameResult = ValidateNspPart(message.Name);
            response.PatronymicResult = ValidateNspPart(message.Patronymic);

            return response;
        }

        private bool ValidateNspPart(string part)
        {
            string pattern = @"^[A-Za-zА-Яа-я]+$";
            return Regex.IsMatch(part, pattern);
        }
    }
}