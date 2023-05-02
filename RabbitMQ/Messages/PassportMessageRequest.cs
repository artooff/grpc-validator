using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Messages
{
    public class PassportMessageRequest : IRabbitMQMessage
    {
        public string PassportInfo;

        public void GetFromByteArray(byte[] byteArray)
        {
            PassportInfo = System.Text.Encoding.Default.GetString(byteArray);
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(PassportInfo);
        }
    }
}
