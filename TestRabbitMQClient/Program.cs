using RabbitMQ;
using RabbitMQ.Messages;
using System;

namespace Validation.PassportValidation
{
    public class Programm
    {
        public static void Main()
        {
            RabbitMQRPCSender<StringMessageRequest, StringMessageResponse> sender = new(null);
            var message = new StringMessageRequest()
            {
                Data = "1234567890"
            };

            var tasksList = new List<Task<StringMessageResponse>>();
            for(int i = 0; i < 10; i++)
            {
                var resultTask = sender.CallAsync(message);
                tasksList.Add(resultTask);
            }
            Task.WaitAll(tasksList.ToArray());

           foreach(var task in tasksList)
            {
                Console.WriteLine(task.Result.ValidationResult);
            }
            Thread.Sleep(20000);
        }
    }
}
