using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Models
{

    enum ValidationStates
    {
        NotChecked,
        Valid,
        Invalid
    }

    public class CardValidationResult
    {
        public NSPValidationResult NspResult { get; set; }
        public bool PassportResult { get; set; }

        public CardValidationResult()
        {
            NspResult = new NSPValidationResult();
        }
    }

    public class NSPValidationResult
    {
        public bool NameResult { get; set; }
        public bool SurnameResult { get; set; }
        public bool PatronymicResult { get; set; }
    }
}
