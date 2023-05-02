using Grpc.Core;
using RabbitMQ;
using RabbitMQ.Messages;
using Validation.MainServer;
using Validation.NSPValidation;
using Validation.PhoneNumberValidation;

namespace Validation.MainServer.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly NSPValidatorClient _nspClient;
        private readonly PhoneNumberValidatorClient _phoneNumberClient;

        private readonly RabbitMQRPCSender<NSPMessageRequest, NSPMessageResponse> _nspValidatorSender;
        private readonly RabbitMQRPCSender<PassportMessageRequest, PassportMessageResponse> _passportValidatorSender;
        private readonly RabbitMQRPCSender<PhoneMessageRequest, PhoneMessageResponse> _phoneValidatorSender;

        public GreeterService(IServiceProvider provider, ILogger<GreeterService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nspClient = new NSPValidatorClient();
            _phoneNumberClient = new PhoneNumberValidatorClient();
            _nspValidatorSender = provider.GetService<RabbitMQRPCSender<NSPMessageRequest, NSPMessageResponse>>()
                ?? new RabbitMQRPCSender<NSPMessageRequest, NSPMessageResponse>(null, logger);

            _passportValidatorSender = provider.GetService<RabbitMQRPCSender<PassportMessageRequest, PassportMessageResponse>>()
                ?? new RabbitMQRPCSender<PassportMessageRequest, PassportMessageResponse>(null, logger);

            _phoneValidatorSender = provider.GetService<RabbitMQRPCSender<PhoneMessageRequest, PhoneMessageResponse>>()
                ?? new RabbitMQRPCSender<PhoneMessageRequest, PhoneMessageResponse>(null, logger);
        }

        public override async Task<RecordsResponse> Validate(RecordsRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = new RecordsResponse();
                foreach(var record in request.Records)
                {

                    var nspRequest = new NSPMessageRequest
                    {
                        Surname = record.Nsp.Surname,
                        Name = record.Nsp.Name,
                        Patronymic = record.Nsp.Patronymic
                    };
                    var nspTask = _nspValidatorSender.CallAsync(nspRequest, context.CancellationToken);

                    var phoneTasks = new List<Task<PhoneMessageResponse>>();
                    foreach(var phoneNumber in record.PhoneNumbers)
                    {
                        var phoneNumberRequest = new PhoneMessageRequest
                        {
                            PhoneNumber = phoneNumber
                        };
                        var phoneNumberTask = _phoneValidatorSender.CallAsync(phoneNumberRequest, context.CancellationToken);
                        phoneTasks.Add(phoneNumberTask);
                    }

                    var passportMessage = new PassportMessageRequest
                    {
                        PassportInfo = record.PassportInfo
                    };
                    var passportTask = _passportValidatorSender.CallAsync(passportMessage);
                    
                   //TODO: tasks to one list
                    await Task.WhenAll(phoneTasks).ConfigureAwait(false);
                    await Task.WhenAll(nspTask).ConfigureAwait(false);
                    await Task.WhenAll(passportTask).ConfigureAwait(false);
                    var currentRecordResponse = new RecordResponse()
                    {
                        Record = record,
                        ValidationResult = new RecordValidationResult
                        {
                            Nsp = new NSPValidationResult
                            {
                                Name = new ValidationResult
                                {
                                    Value = record.Nsp.Name,
                                    IsValid = nspTask.Result.NameResult
                                },
                                Surname = new ValidationResult
                                {
                                    Value = record.Nsp.Surname,
                                    IsValid = nspTask.Result.SurnameResult
                                },
                                Patronymic = new ValidationResult
                                {
                                    Value = record.Nsp.Patronymic,
                                    IsValid = nspTask.Result.PatronymicResult
                                },
                            },
                            PassportData = new ValidationResult
                            {
                                Value = record.PassportInfo,
                                IsValid = passportTask.Result.ValidationResult
                            }
                            
                        }
                    };
                    foreach(var task in phoneTasks)
                    {
                        currentRecordResponse.ValidationResult.PhoneNumbers.Add(new ValidationResult
                        {
                            //Value = task.Result.
                            IsValid = task.Result.ValidationResult
                        });
                    }
                    
                    validationResult.Records.Add(currentRecordResponse);
                }
                
                _logger.LogInformation(nameof(Validate) + " method called");
                return validationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Validate) + " exception");
                return new RecordsResponse();
            }
        }
    }
}