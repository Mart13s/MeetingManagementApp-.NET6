namespace MeetingManagementApp;

public class Interpreter
{

    private readonly ICommandService _commandService;
    private readonly IUserRepository _userRepository;

    public Interpreter(ICommandService commandService)
    {
        if (commandService == null)
            throw new ArgumentNullException("Command service is null.");

        _commandService = commandService;
        _userRepository = commandService.UserRepos;
    }

    /// <summary>
    /// Main interpreter method. This method takes in a string input and launches appropriate commands.
    /// </summary>
    /// <param name="input">Command input string.</param>
    /// <returns>true, if ready for next input. false, to stop interpreting.</returns>
    public bool Interpret(string? input)
    {

        if (input == null) return true;

        string[] tokens = input.Split(' ');
        if (tokens.Length == 0) return true; // No input, continue

        string result = "";

        switch (tokens[0].ToUpper())
        {
            case "REGISTER":
                result = _commandService.RegisterCommand();
                break;

            case "LOGIN":
                if (_userRepository.LoggedIn == "")
                    result = _commandService.LoginCommand(tokens);
                else
                    result = $"Already logged in as {_userRepository.LoggedIn}";
                break;

            case "LOGOUT":
                if (_userRepository.LoggedIn == "") result = "Currently not logged in.";

                else
                {
                    _userRepository.LogOut();    // Redraw console header on logout
                    CommandService.DrawConsoleHeader("");
                }
                break;

            case "LIST":
                result = _commandService.ListCommand(tokens);
                break;

            case "ADD_MEETING":
                result = _commandService.AddMeetingCommand();
                break;

            case "ADD_ATTENDEE":
                result = _commandService.AddAttendeeCommand();
                break;

            case "REMOVE_MEETING":
                result = _commandService.RemoveMeetingCommand();
                break;

            case "REMOVE_ATTENDEE":
                result = _commandService.RemoveAttendeeCommand();
                break;

            case "CLEAR":
            case "CLR":
                try
                {
                    CommandService.DrawConsoleHeader(_userRepository.LoggedIn);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

                return true;

            case "EXIT":
                return false; // If exit is called, we return false to signal the end of interpreting.

            case null:
            case "":
                break;

            default:
                result = "Invalid command.";
                break;
        }

        Console.WriteLine("\n    " + result + "\n"); // Printing out command Success/Error messages to console.
        return true;
    }
}

