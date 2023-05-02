using Validation.MainServer;
using Validation.Models;

namespace Validation.Convertors
{
    public class Convertor
    {
        public static RecordsRequest ConvertToRequest(IEnumerable<Card> cards)
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
                recordRequest.PhoneNumbers.AddRange(card.PhoneNumbers);
                recordRequest.Addresses.AddRange(card.Addresses);

                recordsRequest.Records.Add(recordRequest);
            }

            return recordsRequest;
        }

        public static List<Card> ConvertFromRequest(RecordsRequest request)
        {
            var cards = new List<Card>();

            foreach (var record in request.Records)
            {
                var card = new Card
                {
                    Name = record.Nsp.Name,
                    Surname = record.Nsp.Surname,
                    Patronymic = record.Nsp.Patronymic,
                    BirthDay = record.Birthday.ToDateTime(),
                    Emails = record.Emails.ToList(),
                    Addresses = record.Addresses.ToList(),
                    PhoneNumbers = record.PhoneNumbers.ToList(),
                    PassportInfo = record.PassportInfo
                };
                cards.Add(card);
            }

            return cards;
        }
    }
}