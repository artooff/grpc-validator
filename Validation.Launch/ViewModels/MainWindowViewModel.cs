using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Validation.Client;
using Validation.Models;
using WPF.MVVM;

namespace Validation.Launch.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Card> _cards = new ObservableCollection<Card>();

        public ObservableCollection<Card> Cards { get; set; }
        
        public Card CurrentCard { get; set; }
        public string EmailInput { get; set; }
        public string AddressInput { get; set; }
        public string PhoneNumberInput { get; set; }

        private Lazy<ICommand> _addCommand;
        private Lazy<ICommand> _sendCommand;
        private Lazy<ICommand> _addNewEmailCommand;
        private Lazy<ICommand> _addNewAddressCommand;
        private Lazy<ICommand> _addNewPhoneNumberCommand;
        public ICommand AddCommand => _addCommand.Value;
        public ICommand SendCommand => _sendCommand.Value;
        public ICommand AddNewEmailCommand => _addNewEmailCommand.Value;
        public ICommand AddNewAddressCommand => _addNewAddressCommand.Value;
        public ICommand AddNewPhoneNumberCommand => _addNewPhoneNumberCommand.Value;

        private GreeterClient _greeterClient;

        public MainWindowViewModel()
        {
            _greeterClient = new GreeterClient();

            _addCommand = new Lazy<ICommand>(() => new RelayCommand(_ => AddRecord()));
            _sendCommand = new Lazy<ICommand>(() => new RelayCommand(_ => SendRecords()));
            _addNewEmailCommand = new Lazy<ICommand>(() => new RelayCommand(_ => AddNewEmail()));
            _addNewAddressCommand = new Lazy<ICommand>(() => new RelayCommand(_ => AddNewAdress()));
            _addNewPhoneNumberCommand = new Lazy<ICommand>(() => new RelayCommand(_ => AddNewPhoneNumber()));

            Cards = new ObservableCollection<Card>();
            CurrentCard = new Card
            {
                Name = "Иван",
                Surname = "Иванов",
                Patronymic = "Иванович",
                PassportInfo = "121212"
            };
            CurrentCard.Emails.Add("123");
            CurrentCard.Emails.Add("432423");
            CurrentCard.Addresses.Add("1212121231");
            CurrentCard.Addresses.Add("1231542534");
            CurrentCard.PhoneNumbers.Add(new PhoneNumber
            {
                Number = "+79721237483"
            });
            CurrentCard.PhoneNumbers.Add(new PhoneNumber
            {
                Number = "99ff90g"
            });

        }

        private void AddRecord()
        {
            Cards.Add((Card)CurrentCard.Clone());
        }

        private async void SendRecords()
        {
            var recordsResponse = await _greeterClient.ValidateCardsAsync(Cards);
            //Cards = (ObservableCollection<Card>)resultCollection;

            var resultCollection = new ObservableCollection<Card>();
            foreach (var record in recordsResponse.Records)
            {
                var card = new Card
                {
                    Name = record.Record.Nsp.Name,
                    Surname = record.Record.Nsp.Surname,
                    Patronymic = record.Record.Nsp.Patronymic,
                    BirthDay = (record.Record.Birthday).ToDateTime(),
                    Addresses = new ObservableCollection<string>((record.Record.Addresses).ToList<string>()),
                    Emails = new ObservableCollection<string>((record.Record.Emails).ToList<string>()),
                    PassportInfo = record.Record.PassportInfo
                };
                card.ValidationResult.NspResult.NameResult = record.ValidationResult.Nsp.Name.IsValid;
                card.ValidationResult.NspResult.SurnameResult = record.ValidationResult.Nsp.Surname.IsValid;
                card.ValidationResult.NspResult.PatronymicResult = record.ValidationResult.Nsp.Patronymic.IsValid;
                card.PhoneNumbers = new ObservableCollection<PhoneNumber>();
                foreach (var number in record.ValidationResult.PhoneNumbers)
                {
                    card.PhoneNumbers.Add(new PhoneNumber
                    {
                        Number = number.Value,
                        ValidationResult = number.IsValid
                    });
                }

                card.ValidationResult.PassportResult = record.ValidationResult.PassportData.IsValid;

                resultCollection.Add(card);
            }

            Cards.Clear();
            foreach(var card in resultCollection)
            {
                Cards.Add(card);
            }
        }

        private void AddNewEmail()
        {
            if (EmailInput != null)
                CurrentCard.Emails.Add(EmailInput);
        }

        private void AddNewAdress()
        {
            if (AddressInput != null)
                CurrentCard.Addresses.Add(AddressInput);
        }

        private void AddNewPhoneNumber()
        {
            if (PhoneNumberInput != null)
                CurrentCard.PhoneNumbers.Add(new PhoneNumber
                {
                    Number = PhoneNumberInput
                });
        }
    }
}
