using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Validation.MainServer;
using Validation.Models;

namespace Validation.Client
{
    public class GreeterClient
    {
        private readonly ILogger<GreeterClient> _logger;
        private readonly Greeter.GreeterClient _client;

        public GreeterClient(ILogger<GreeterClient> logger = null)
        {
            _logger =  logger;

            var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:7205",
                new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });

            _client = new Greeter.GreeterClient(channel);
            
        }

        public async Task<RecordsResponse> ValidateCardsAsync(IEnumerable<Card> cards, CancellationToken token = default)
        {
            try
            {
                var recordsRequest = new RecordsRequest();

                foreach (var card in cards)
                {
                    var recordRequest = new RecordRequest
                    {
                        Nsp = new NSP
                        {
                            Name = card.Name,
                            Surname = card.Surname,
                            Patronymic = card.Patronymic
                        },
                        Birthday = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(card.BirthDay.ToUniversalTime()),
                        PassportInfo = card.PassportInfo

                    };
                    recordRequest.Emails.AddRange(card.Emails);
                    recordRequest.PhoneNumbers.AddRange(card.PhoneNumbers.Select(v => v.Number));
                    recordRequest.Addresses.AddRange(card.Addresses);
                    
                    recordsRequest.Records.Add(recordRequest);

                }

                var recordsResponse = await _client.ValidateAsync(recordsRequest, new Grpc.Core.CallOptions(cancellationToken: token));
                return recordsResponse;
                //Task.WhenAll(recordsResponseTask);
                //var resultCollection = new ObservableCollection<Card>();
                //foreach(var record in recordsResponse.Records)
                //{
                //    var card = new Card();

                //    card.Name = record.Record.Nsp.Name;
                //    card.Surname = record.Record.Nsp.Surname;
                //    card.Patronymic = record.Record.Nsp.Patronymic;
                //    card.BirthDay = (record.Record.Birthday).ToDateTime();
                //    card.Addresses = new ObservableCollection<string>((record.Record.Addresses).ToList<string>());
                //    card.Emails = new ObservableCollection<string>((record.Record.Emails).ToList<string>());
                //    card.PhoneNumbers = new ObservableCollection<string>((record.Record.PhoneNumbers).ToList<string>());
                //    card.PassportInfo = record.Record.PassportInfo;
                //    card.ValidationResult.NspResult.NameResult = record.ValidationResult.Nsp.Name.IsValid;
                //    card.ValidationResult.NspResult.NameResult = record.ValidationResult.Nsp.Surname.IsValid;
                //    card.ValidationResult.NspResult.NameResult = record.ValidationResult.Nsp.Patronymic.IsValid;
                //}

                //return resultCollection;
                
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        
    }
}