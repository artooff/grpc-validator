using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Messages
{
    public class NSPMessageRequest : IRabbitMQMessage
    {
        public string Surname;
        public string Name;
        public string Patronymic;
        public void GetFromByteArray(byte[] byteArray)
        {
            var concatString = byteArray.ToString();
            string[] subs = concatString.Split(':');
            Surname = subs[0];
            Name = subs[1];
            Patronymic = subs[2];
        }

        public byte[] ToByteArray()
        {
            var concatString = Surname + ":" + Name + ":" + Patronymic;
            return Encoding.UTF8.GetBytes(concatString);
        }

        
    }
}
