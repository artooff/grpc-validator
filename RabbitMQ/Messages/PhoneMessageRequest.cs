using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Messages
{
    public class PhoneMessageRequest : IRabbitMQMessage
    {
        public string PhoneNumber;

        public void GetFromByteArray(byte[] byteArray)
        {
            PhoneNumber = System.Text.Encoding.Default.GetString(byteArray);
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(PhoneNumber);
        }
    }
}
