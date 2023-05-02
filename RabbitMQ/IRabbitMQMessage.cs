using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public interface IRabbitMQMessage
    {
        public byte[] ToByteArray();
        public void GetFromByteArray(byte[] byteArray);
    }
}
