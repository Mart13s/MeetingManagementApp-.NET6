using System.Text;
using System.Text.RegularExpressions;

namespace MeetingManagementApp
{
    public interface ICommandService
    {
        IMeetingRepository MeetingRepos { get; }
        IUserRepository UserRepos { get; }

        string AddAttendeeCommand();
        string AddMeetingCommand();
        ICollection<Meeting> FilterMeetings(ICollection<Meeting> meetings, string filters);
        string ListCommand(string[] tokens);
        string LoginCommand(string[] tokens);
        void PrintMeetingList(ICollection<Meeting> meetings);
        string RegisterCommand();
        string RemoveAttendeeCommand();
        string RemoveMeetingCommand();
    }

    public class CommandService : ICommandService
    {
        public IUserRepository UserRepos { get; private set; }
        public IMeetingRepository MeetingRepos { get; private set; }



        public CommandService(IUserRepository userRepository, IMeetingRepository meetingRepository)
        {

            if (userRepository != null) UserRepos = userRepository;
            else UserRepos = UserRepository.Instance;

            if (meetingRepository != null) MeetingRepos = meetingRepository;
            else MeetingRepos = MeetingRepository.Instance;

        }


        /// <summary>
        /// Command to handle user registration. Asks for a unique username, alphanumeric password and a repeat password.
        /// </summary>
        /// <returns>Error/Success message string.</returns>
        public string RegisterCommand()
        {
            // Username handling
            Console.WriteLine("\n    Please enter a unique username:");
            Console.Write("    ");
            string? username = Console.ReadLine();

            if (username == null || username == "")
                return "Username may not be empty!";

            if (!Regex.IsMatch(username, "^([A-Za-zĄ-Žą-ž]+)$"))
                return "Username may only contain letters!";

            if (username.Length > 30)
                return "Username may not be longer than 30 characters!";

            // Password handling
            Console.WriteLine("\n    Please enter a password (must be atleast 3 chars and contain a number and a letter):");
            Console.Write("    ");
            string? password = Console.ReadLine();

            if (password == null || password == "")
                return "Password may not be empty!";

            if (!Regex.IsMatch(password, "^(?=.*[0-9])(?=.*[a-zA-Zą-žĄ-Ž])([a-zA-Zą-žĄ-Ž0-9]+)$"))
                return "Password must contain a number and a letter";

            if (password.Length < 3)
                return "Password must contain atleast 3 characters";

            // Repeat password handling

            Console.WriteLine("\n    Please re-enter password:");
            Console.Write("    ");
            string? repeat = Console.ReadLine();
            if (string.Compare(password, repeat) != 0)
                return "Passwords don't match";


            // Register user
            bool registerResult = UserRepos.Register(username, password);

            return registerResult ? "Registration successful." : "User already exists.";

        }

        /// <summary>
        /// Login handling command. Using a stored salt we generate a password hash and then check,
        /// if they equate.
        /// </summary>
        /// <param name="tokens"> Input is an array of strings formatted as ["login", "username", "password"]</param>
        /// <returns>Success/Error message string</returns>
        public string LoginCommand(string[] tokens)
        {
            if (tokens.Length != 3) return "Please use LOGIN [username] [password]";

            if (UserRepos.Login(tokens[1], tokens[2]))
            {
                DrawConsoleHeader(UserRepos.LoggedIn);
                return $"Successfully logged in as {UserRepos.LoggedIn}";
            }
            else return "Invalid credentials.";

        }


        /// <summary>
        /// This method handles meeting list table printing.
        /// </summary>
        /// <param name="meetings">A collection of meetings.</param>
        public void PrintMeetingList(ICollection<Meeting> meetings)
        {

            Console.Write('\n');

            if (meetings == null || meetings.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("    No meetings found.\n    ");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            // Getting current windowWidth minus the padding
            int viewWidth = Console.WindowWidth >= 3 ? Console.WindowWidth - 3 : 0;
            int widthPercent = viewWidth / 100;

            // Border and dataline setup
            string tableBorder = new string('-', viewWidth);
            string emptyLine = string.Format(" |{0, -" + (viewWidth - 2) + "}|", "");
            string tableHeaderTemplate = string.Format(
                " {0, -" + widthPercent * 28 + "} | {1, -" + widthPercent * 25 + "} | {2, -" + widthPercent * 24 + "} | {3, -" + widthPercent * 24 + "}",
                "", "", "", "");

            string tableDataEmpty = string.Format("|{0, -" + (viewWidth - 2) + "}|", tableHeaderTemplate);
            string tableHeader, tableHeader2, tableData;


            // Iterating through meeting collection and printing out values to console
            foreach (Meeting m in meetings)
            {

                // Mark start for better visibility
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(' ' + tableBorder);
                Console.WriteLine(' ' + tableDataEmpty);
                Console.ForegroundColor = ConsoleColor.White;

                // Meeting info printing
                tableHeader = string.Format(" {0, -" + widthPercent * 28 + "} | {1, -" + widthPercent * 25 + "} | {2, -" + widthPercent * 24 + "} | {3, -" + widthPercent * 24 + "}",
                              "Meeting name: " + m.MeetingName,
                              "Description: " + m.Description,
                              "Category: " + m.Category,
                              "Type: " + m.MeetType);

                Console.WriteLine(string.Format(" |{0, -" + (viewWidth - 2) + "}|", tableHeader));

                // Drawing second header
                tableHeader2 = string.Format(" {0, -" + widthPercent * 28 + "} | {1, -" + widthPercent * 25 + "} | {2, -" + widthPercent * 24 + "} | {3, -" + widthPercent * 24 + "}",
                              "Responsible person: " + m.ResponsiblePerson,
                              "Attendee count: " + m.Attendees.Count.ToString(),
                              "Starts: " + m.From.ToString("yyyy-MM-dd HH:mm"),
                              "Ends: " + m.To.ToString("yyyy-MM-dd HH:mm"));

                Console.WriteLine(string.Format(" |{0, -" + (viewWidth - 2) + "}|", tableHeader2));
                Console.WriteLine(' ' + tableDataEmpty);

                // Attendee info printing
                int i = 0;
                foreach (UserMeeting attendee in m.Attendees)
                {
                    tableData = string.Format(" {0, -" + widthPercent * 28 + "} | {1, -" + widthPercent * 25 + "} | {2, -" + widthPercent * 24 + "} | {3, -" + widthPercent * 24 + "}",
                              "Index: " + (++i),
                              "Name: " + attendee.Name,
                              "  From: " + attendee.TimeFrom.ToString("yyyy-MM-dd HH:mm"),
                              "  To: " + attendee.TimeTo.ToString("yyyy-MM-dd HH:mm"));

                    Console.WriteLine(string.Format(" |{0, -" + (viewWidth - 2) + "}|", tableData));
                }
                Console.WriteLine(' ' + tableDataEmpty);


            }

            // Mark end for better visibility
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(' ' + tableDataEmpty);
            Console.WriteLine(' ' + tableBorder);
            Console.ForegroundColor = ConsoleColor.White;
        }


        /// <summary>
        /// This command handles adding a new meeting.
        /// </summary>
        /// <returns>Sucecess/Error message</returns>
        public string AddMeetingCommand()
        {
            // Meeting name input handling
            Console.WriteLine("\n    Please enter a unique meeting name:");
            Console.Write("    ");
            string? name = Console.ReadLine();
            if (name == null || name == "") return "Meeting name can't be empty";

            // Responsible person input handling
            Console.WriteLine("\n    Please enter user responsible:");
            Console.Write("    ");
            string? username = Console.ReadLine();
            if (username == null || username == "") return "Responsible user must have a name";

            User user = UserRepos.GetUser(username);
            if (user == null) return $"User with username {username} does not exist.";

            // Description input handling
            Console.WriteLine("\n    Please enter a meeting description:");
            Console.Write("    ");
            string? desc = Console.ReadLine();
            if (desc == null || desc == "") return "Meeting description can't be empty";

            // Meeting category input handling
            Console.WriteLine("\n    Please enter a meeting category (CodeMonkey,Hub,Short,TeamBuilding):");
            Console.Write("    ");

            string? category = Console.ReadLine();
            if (category == null || category == "") return "Invalid meeting category";

            MeetingCategory meetingCategory;
            try
            {
                meetingCategory = (MeetingCategory)Enum.Parse(typeof(MeetingCategory), category, true);
            }
            catch
            {
                return "Invalid meeting category";
            }

            // Meeting type input handling
            Console.WriteLine("\n    Please enter a meeting type (Live, InPerson):");
            Console.Write("    ");

            string? meetingTypeString = Console.ReadLine();
            if (meetingTypeString == null || meetingTypeString == "") return "Invalid meeting type";

            MeetingType meetingType;

            try
            {
                meetingType = (MeetingType)Enum.Parse(typeof(MeetingType), meetingTypeString, true);
            }
            catch
            {
                return "Invalid meeting type";
            }

            // Meeting start input handling
            Console.WriteLine("\n    Please enter meeting start time (Format: yyyy-MM-dd HH:mm)");
            Console.Write("    ");

            string? meetingStart = Console.ReadLine();

            DateTime from;

            try
            {
                from = DateTime.ParseExact(meetingStart, "yyyy-MM-dd HH:mm", null);
            }

            catch
            {
                from = DateTime.Now; // If date parsing fails, we set meeting to start now
            }

            // Meeting End input handling
            Console.WriteLine("\n    Please enter meeting end time (Format: yyyy-MM-dd HH:mm)");
            Console.Write("    ");

            string? meetingEnd = Console.ReadLine();

            DateTime to;
            try
            {
                to = DateTime.ParseExact(meetingEnd, "yyyy-MM-dd HH:mm", null);
            }

            catch
            {
                to = DateTime.Now.AddYears(1); //  If date parsing fails, we set meeting to end a year later
            }


            // Calling controller method to create a new meeting
            try
            {
                MeetingRepos.AddMeeting(name, user, desc, meetingCategory, meetingType, from, to);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Meeting added successfully.";
        }


        /// <summary>
        /// Meeting removal method. Checks, if meeting exists and if the user is authorized to delete it.
        /// </summary>
        /// <returns>Success/Error message string</returns>
        public string RemoveMeetingCommand()
        {
            // Meeting name handling
            Console.WriteLine("\n    Please enter the name of meeting you'd like to remove:");
            Console.Write("    ");

            string? meetingName = Console.ReadLine();
            if (meetingName == null || meetingName == "") return "Meeting name may not be empty";

            try
            {
                MeetingRepos.RemoveMeeting(meetingName);
            }

            catch (Exception ex)
            {
                // Catching unauthorized removal or attempted removal of non-existant meetings
                return ex.Message;
            }

            return "Meeting removed successfully.";
        }


        /// <summary>
        /// Add attendee method adds an existing user to an existing meeting, if:
        /// a) user is not busy at that meeting time
        /// and
        /// b) user is not already in this meeting
        /// </summary>
        /// <returns>Success/Error message string</returns>
        public string AddAttendeeCommand()
        {
            // Meeting name input handling
            Console.WriteLine("\n    Please enter the name of the meeting:");
            Console.Write("    ");

            string? meetingName = Console.ReadLine();
            if (meetingName == null || meetingName == "") return "Meeting name may not be empty";

            // Attendee name input handling
            Console.WriteLine("\n    Please enter the name of the attendee:");
            Console.Write("    ");

            string? attendeeName = Console.ReadLine();
            if (attendeeName == null || attendeeName == "") return "Attendee name may not be empty";

            // Date from input handling
            Console.WriteLine("\n    Please enter meeting start time (Format: yyyy-MM-dd HH:mm)");
            Console.Write("    ");

            string? meetingStart = Console.ReadLine();

            DateTime from;

            try
            {
                from = DateTime.ParseExact(meetingStart, "yyyy-MM-dd HH:mm", null);
            }

            catch
            {
                from = DateTime.Now; // If parsing fails, we set the attendance to now.
            }

            // Date to input handling
            Console.WriteLine("\n    Please enter meeting end time (Format: yyyy-MM-dd HH:mm)");
            Console.Write("    ");

            string? meetingEnd = Console.ReadLine();

            DateTime end;

            try
            {
                end = DateTime.ParseExact(meetingEnd, "yyyy-MM-dd HH:mm", null);
            }

            catch
            {
                end = DateTime.Now.AddYears(1); // If parsing fails, we set the attendance to a year later.
            }


            // Adding attendee to meeting
            try
            {
                MeetingRepos.AddAttendee(meetingName, attendeeName, from, end);
            }

            catch (Exception ex)
            {
                // Handling errors - user is already in meeting, user is busy and user or meeting non-existance.
                return ex.Message;
            }

            return "Attendee added successfully.";
        }


        /// <summary>
        /// This method handles removal of an attendee from a meeting, if that user is not reponsible for that meeting
        /// Requests the user to input meeting name and username of user to be removed.
        /// </summary>
        /// <returns>Success/Error message</returns>
        public string RemoveAttendeeCommand()
        {
            // Meeting name input handling
            Console.WriteLine("\n    Please enter the name of the meeting:");
            Console.Write("    ");

            string? meetingName = Console.ReadLine();
            if (meetingName == null || meetingName == "") return "Meeting name may not be empty";

            // Attendee name input handling
            Console.WriteLine("\n    Please enter the name of the attendee:");
            Console.Write("    ");

            string? attendeeName = Console.ReadLine();
            if (attendeeName == null || attendeeName == "") return "Attendee name may not be empty";

            try
            {
                MeetingRepos.RemoveAttendee(meetingName, attendeeName);
            }

            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Attendee removal successful.";
        }

        /// <summary>
        /// This command checks, if filters are needed to process meeting list, calls filtering method, if needed.
        /// Then calls method "PrintMeetingList" to printout meeting list to console.
        /// </summary>
        /// <param name="tokens">A string of an inputted List command</param>
        /// <returns>Success/Error message</returns>
        public string ListCommand(string[] tokens)
        {

            ICollection<Meeting> meetings = MeetingRepos.GetMeetings();

            if (tokens.Length > 1
                && (tokens[1].ToUpper().Equals("-FILTER") || tokens[1].ToUpper().Equals("-FILTERS") || tokens[1].ToUpper().Equals("-F")))
            {

                // Using string builder to reform a string from filtername tokens
                StringBuilder sb = new StringBuilder();
                for (int i = 2; i < tokens.Length; i++)
                {
                    sb.Append(tokens[i].ToUpper() + ' ');
                }

                // Passing meeting info and filtername token string to filtering method
                meetings = FilterMeetings(meetings, sb.ToString());
            }

            // Printing out resulting meeting list
            PrintMeetingList(meetings);
            return "";
        }


        /// <summary>
        /// This method filters out a collection of meetings by using filters from filter string
        /// and parsed user inputs.
        /// </summary>
        /// <param name="meetings">Initial meeting list.</param>
        /// <param name="filters">Filter string, e.g.: ["DESC RESP DATE_TO"]</param>
        /// <returns>A filtered out Meeting collection.</returns>
        public ICollection<Meeting> FilterMeetings(ICollection<Meeting> meetings, string filters)
        {
            if (meetings == null || meetings.Count == 0 || filters == null) return meetings;

            filters = filters.ToUpper();


            Console.WriteLine("");

            // Probably should just use a switch or something smarter, don't judge me too hard pls
            // I use regex to detect, if filter is in filters string and then handle input of a parameter
            // If parsing is incorrect or input is empty that filter is skipped

            if (Regex.IsMatch(filters, @"(^|\b)(DESC|DESCRIPTION)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the description to filter by:\n    ");
                string? description = Console.ReadLine();
                if (description == null || description == "") Console.WriteLine("     Description left empty, skipping...");
                else
                {
                    meetings = meetings.Where(x => x.Description.Contains(description, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            if (Regex.IsMatch(filters, @"(^|\b)(RESP|RESPONSIBLE)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the responsible person's name to filter by:\n    ");
                string? responsible = Console.ReadLine();
                if (responsible == null || responsible == "") Console.WriteLine("     Responsible person left empty, skipping...");
                else
                {
                    meetings = meetings.Where(x => x.ResponsiblePerson.Contains(responsible, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            if (Regex.IsMatch(filters, @"(^|\b)(CAT|CATEGORY)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the category to filter by:\n    ");
                string? category = Console.ReadLine();
                if (category == null || category == "") Console.WriteLine("    Category left empty, skipping...");
                else
                {
                    MeetingCategory meetingCategory;

                    try
                    {
                        meetingCategory = (MeetingCategory)Enum.Parse(typeof(MeetingCategory), category, true);

                        meetings = meetings.Where(x => x.Category == meetingCategory).ToList();
                    }
                    catch
                    {
                        Console.WriteLine("    Invalid category skipping...");
                    }
                }
            }


            if (Regex.IsMatch(filters, @"(^|\b)(TYPE)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the meeting type to filter by:\n    ");
                string? type = Console.ReadLine();
                if (type == null || type == "") Console.WriteLine("    Type left empty, skipping...");
                else
                {
                    MeetingType meetingType;

                    try
                    {
                        meetingType = (MeetingType)Enum.Parse(typeof(MeetingType), type, true);

                        meetings = meetings.Where(x => x.MeetType == meetingType).ToList();
                    }
                    catch
                    {
                        Console.WriteLine("     Invalid meeting type skipping...");
                    }
                }
            }

            if (Regex.IsMatch(filters, @"(^|\b)(DATE_FROM|DATEFROM)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the date from to filter by (format yyyy-MM-dd HH:mm):\n    ");
                string? dateFrom = Console.ReadLine();
                if (dateFrom == null || dateFrom == "") Console.WriteLine("      Date from left empty, skipping...");
                else
                {
                    try
                    {
                        DateTime date = DateTime.ParseExact(dateFrom, "yyyy-MM-dd HH:mm", null);
                        meetings = meetings.Where(x => x.From >= date).ToList();
                    }
                    catch
                    {
                        Console.WriteLine("     Invalid date entered skipping...");
                    }
                }
            }

            if (Regex.IsMatch(filters, @"(^|\b)(DATE_TO|DATETO)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("     Please enter the date_to to filter by (format yyyy-MM-dd HH:mm):\n    ");
                string? dateTo = Console.ReadLine();
                if (dateTo == null || dateTo == "") Console.WriteLine("     Date to left empty, skipping...");
                else
                {
                    try
                    {
                        DateTime date = DateTime.ParseExact(dateTo, "yyyy-MM-dd HH:mm", null);
                        meetings = meetings.Where(x => x.To <= date).ToList();
                    }
                    catch
                    {
                        Console.WriteLine("    Invalid date entered skipping...");
                    }
                }
            }

            if (Regex.IsMatch(filters, @"(^|\b)(ATTENDEES_FROM|ATTENDEESFROM)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the attendee count from to filter by:\n    ");
                int attendeesFrom = 0;
                bool result = int.TryParse(Console.ReadLine(), out attendeesFrom);
                if (!result) Console.WriteLine("    Invalid number entered, skipping...");
                else
                {
                    meetings = meetings.Where(x => x.Attendees.Count() >= attendeesFrom).ToList();
                }
            }


            if (Regex.IsMatch(filters, @"(^|\b)(ATTENDEES_TO|ATTENDEESTO)($|\b)", RegexOptions.IgnoreCase))
            {
                Console.Write("    Please enter the max attendee count to filter by:\n    ");
                int attendeeTo = 0;
                bool result = int.TryParse(Console.ReadLine(), out attendeeTo);
                if (!result) Console.WriteLine("     Invalid number entered, skipping...");
                else
                {
                    meetings = meetings.Where(x => x.Attendees.Count() <= attendeeTo).ToList();
                }
            }

            return meetings;
        }

        /// <summary>
        /// This method draws a console header.
        /// </summary>
        /// <param name="username">Currently logged in user. Leave empty, if not logged in.</param>
        public static void DrawConsoleHeader(string username = "")
        {
            Console.Clear();

            int viewWidth = Console.WindowWidth >= 3 ? Console.WindowWidth - 3 : 0;
            string headerEmptyLine = string.Format(" |{0, " + (viewWidth - 2) + "}|", "");


            // Header top edge

            Console.Write(' ');
            Console.Write(new string('-', viewWidth));
            Console.Write(" \n");

            // Header top padding

            Console.WriteLine(headerEmptyLine);

            // Draw login info

            Console.Write(" |");

            if (username != null && username != "") // If currently logged in, draw logged in header
            {

                // Drawing user info

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("{0, -" + (viewWidth - 2) + " }", $" Logged in as: {username} (type LOGOUT to log out or EXIT to exit)");
                Console.ForegroundColor = ConsoleColor.White;
            }

            else // If not logged in, draw login instructions.
            {
                // Drawing user info

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("{0, -" + (viewWidth - 2) + " }",
                    " Currently not logged in. Type LOGIN [username] [password], REGISTER or EXIT.");
                Console.ForegroundColor = ConsoleColor.White;

            }

            Console.Write("|\n");

            // Drawing command list
            Console.WriteLine(headerEmptyLine);

            Console.Write(" |");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("{0, -" + (viewWidth - 2) + " }",
                " Hint: All preloaded accounts have the password -> pass1");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|");

            Console.Write(" |");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("{0, -" + (viewWidth - 2) + "}",
        " For test account access type: LOGIN test pass1");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|");

            Console.Write(" |");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("{0, -" + (viewWidth - 2) + "}",
        " Type LIST to get an unfiltered meeting list. You can use CLEAR or CLR to clear the console.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|");

            Console.Write(" |");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("{0, -" + (viewWidth - 2) + "}",
        " Commands are NOT case sensitive, though input variables might be.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|");

            Console.WriteLine(" |{0, -" + (viewWidth - 2) + " }|", "");
            Console.WriteLine(" |{0, -" + (viewWidth - 2) + " }|",
                " Available commands: ADD_MEETING, REMOVE_MEETING, ADD_ATTENDEE, REMOVE_ATTENDEE, LIST ( -FILTERS filtername ... )");
            Console.WriteLine(" |{0, -" + (viewWidth - 2) + " }|",
                " Filter names: DESC DESCRIPTION RESP RESPONSIBLE CAT CATEGORY TYPE DATE_FROM DATE_TO ATTENDEES_FROM ATTENDEES_TO");

            Console.Write(" |");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0, -" + (viewWidth - 2) + "}",
        " Filter command example: LIST -filters desc resp category");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|");


            // Bottom padding

            Console.WriteLine(headerEmptyLine);

            // Bottom edge

            Console.Write(' ');
            Console.Write(new string('-', viewWidth));
            Console.Write(" \n");
        }
    }
}
