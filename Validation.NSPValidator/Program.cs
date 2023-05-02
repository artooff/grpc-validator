using Validation.NSPRabbitValidator;

namespace Validation.PassportValidator;


public class Programm
{
    public static void Main(string[] args)
    {
        var service = new NSPValidatorService(null);
        Console.WriteLine("NSP validator service started...");
        while (true)
        {

        }
    }
}

