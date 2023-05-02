using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Messages
{
    public class PhoneMessageResponse : IRabbitMQMessage
    {
        private bool _validationResult;

        public bool ValidationResult => _validationResult;
        public void GetFromByteArray(byte[] byteArray)
        {
            _validationResult = BitConverter.ToBoolean(byteArray, 0);
        }

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(_validationResult);
        }

        public void SetResult(bool result)
        {
            _validationResult = result;
        }
    }
}
