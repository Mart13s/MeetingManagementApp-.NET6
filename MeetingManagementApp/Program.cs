using MeetingManagementApp;
using System.Text;

namespace MeetingManagementApp;

class Program
{
    static void Main()
    {

        // SETUP CONSOLE
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        // SETUP VARIABLES
        IUserRepository userRepository = UserRepository.Instance;
        IMeetingRepository meetingRepository = MeetingRepository.Instance;
        ICommandService commandService = new CommandService(userRepository, meetingRepository);

        // INTERPRET COMMANDS
        Interpreter interpreter = new Interpreter(commandService);
        string? input = "";

        CommandService.DrawConsoleHeader("");

        do
        {
            Console.Write("\n    Please enter a command:\n    ");
            input = Console.ReadLine();

        } while (interpreter.Interpret(input));


    }
}