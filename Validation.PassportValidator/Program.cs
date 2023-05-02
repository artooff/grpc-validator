using Validation.PassportRabbitValidator;

namespace Validation.PassportValidator;


public class Programm
{
    public static void Main(string[] args)
    {
        var service = new PassportValidatorService(null);
        Console.WriteLine("Passport validator service started...");
        while (true)
        {

        }
    }
}

