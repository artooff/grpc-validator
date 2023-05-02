using Grpc.Net.Client;
using Validation.MainServer;

namespace TestClient
{
    public class TestClientClass
    {
        static async Task Main(string[] args)
        {
            var name = "Andrey";
            using var channel = GrpcChannel.ForAddress("https://localhost:7205");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

            Console.WriteLine(reply.Message);
        }
    }
}
