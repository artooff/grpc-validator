using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Models
{
    public class Card
    {
        private string _name;
        private string _surname;
        private string _patrynomic;
        private DateTime _birthDay;
        private ObservableCollection<string> _emails = new ObservableCollection<string>();
        private ObservableCollection<PhoneNumber> _phoneNumbers = new ObservableCollection<PhoneNumber>();
        private ObservableCollection<string> _addresses = new ObservableCollection<string>();
        private string _passportInfo;

        private CardValidationResult _validationResult;
        public Card()
        {
            _validationResult = new CardValidationResult
            {
                NspResult = new NSPValidationResult()
            };
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Surname
        {
            get => _surname;
            set => _surname = value;
        }

        public string Patronymic
        {
            get => _patrynomic;
            set => _patrynomic = value;
        }

        public DateTime BirthDay
        {
            get => _birthDay;
            set => _birthDay = value;
        }

        public ObservableCollection<string> Emails
        {
            get => _emails;
            set => _emails = value;
        }

        public ObservableCollection<PhoneNumber> PhoneNumbers
        {
            get => _phoneNumbers;
            set => _phoneNumbers = value;
        }

        public ObservableCollection<string> Addresses
        {
            get => _addresses;
            set => _addresses = value;
        }

        public string PassportInfo
        {
            get => _passportInfo;
            set => _passportInfo = value;
        }

        public CardValidationResult ValidationResult
        {
            get => _validationResult;
            set => _validationResult = value;
        }
        public object Clone()
        {
            var other = (Card)MemberwiseClone();
            other.Emails = new ObservableCollection<string>(Emails);
            other.Addresses = new ObservableCollection<string>(Addresses);
            other.PhoneNumbers = new ObservableCollection<PhoneNumber>(PhoneNumbers);
            
            return other;
        }
            
    }
}
