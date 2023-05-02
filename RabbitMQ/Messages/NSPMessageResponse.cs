using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Messages
{
    public class NSPMessageResponse : IRabbitMQMessage
    {
        public bool SurnameResult { get; set; }
        public bool NameResult { get; set; }
        public bool PatronymicResult { get; set; }

        public void GetFromByteArray(byte[] byteArray)
        {
            var partsArray = ConvertByteToBoolArray(byteArray[0]);
            try
            {
                SurnameResult = partsArray[0];
                NameResult = partsArray[1];
                PatronymicResult = partsArray[2];
            }
            catch(IndexOutOfRangeException ex)
            {
                throw ex; 
            }
        }

        public byte[] ToByteArray()
        {
            var array = new byte[1];
            var partsArray = new bool[] {SurnameResult, NameResult, PatronymicResult};
            array[0] = ConvertBoolArrayToByte(partsArray);

            return array;
        }

        private byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        private static bool[] ConvertByteToBoolArray(byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) != 0;

            // reverse the array
            Array.Reverse(result);

            return result;
        }
    }
}
