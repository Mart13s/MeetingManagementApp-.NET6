using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeetingManagementApp;
using System.IO;

namespace MeetingManagementAppTests.CommandServiceTests
{
    [TestClass]
    public class CommandServiceTests
    {

        private static ICommandService _commandService;
        private static IUserRepository _userRepository;
        private static IMeetingRepository _meetingRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) // Setup
        {
            if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");

            _userRepository = UserRepository.Instance;
            _meetingRepository = MeetingRepository.Instance;
            _commandService = new CommandService(_userRepository, _meetingRepository);

            _userRepository.Register("existingUser", "pass1");
            _userRepository.Register("2existingUser", "pass1");
            _userRepository.Register("2existingUser2", "pass1");
            _userRepository.Register("2existingUser3", "pass1");
            _userRepository.Register("2existingUser4", "pass1");
            _userRepository.Register("2existingUser5", "pass1");
            _userRepository.Register("2existingUser6", "pass1");
            _userRepository.Register("2existingUser7", "pass1");
            _userRepository.Register("2existingUser8", "pass1");
            _userRepository.Register("2existingUser9", "pass1");
            _userRepository.Register("3existingUser1", "pass1");
            _userRepository.Register("busyUser", "pass1");
            _meetingRepository.AddMeeting("busyMeeting", _userRepository.GetUser("busyUser"), "desc",
                                          MeetingCategory.Hub, MeetingType.Live, DateTime.MinValue, DateTime.MaxValue);

            User u = _userRepository.GetUser("2existingUser");
            _meetingRepository.AddMeeting("existingMeeting2", u, "Desc", MeetingCategory.CodeMonkey, MeetingType.InPerson,
                                          DateTime.Parse("2000-01-01"), DateTime.Parse("2001-01-01"));
            _meetingRepository.AddAttendee("existingMeeting2", "2existingUser7", DateTime.Parse("2000-01-01"), DateTime.Parse("2001-01-01"));
        }

        [ClassCleanup]
        public static void ClassCleanup() // Teardown
        {
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");
        }

        [TestMethod]
        public void RegisterCommandSuccessTest()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Registration successful.");

        }

        [TestMethod]
        public void RegisterCommandEmptyUsernameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Username may not be empty!");
        }

        [TestMethod]
        public void RegisterCommandInvalidUsernameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qqq1111!!!\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Username may only contain letters!");
        }

        [TestMethod]
        public void RegisterCommandUsernameTooLongTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qwertyqwertyqwertyqwertyqwertyqw\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Username may not be longer than 30 characters!");
        }

        [TestMethod]
        public void RegisterCommandEmptyPasswordTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qwerty\n\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Password may not be empty!");
        }

        [TestMethod]
        public void RegisterCommandInvalidPasswordTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qwerty\nPASS!!!!\nPASS!!!!\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Password must contain a number and a letter");
        }

        [TestMethod]
        public void RegisterCommandPasswordTooShortTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qwerty\np2\np2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Password must contain atleast 3 characters");
        }

        [TestMethod]
        public void RegisterCommandPasswordsDontMatchTest()
        {
            var output = new StringWriter();
            var input = new StringReader("qwerty\npass1\npass2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "Passwords don't match");
        }

        [TestMethod]
        public void RegisterCommandUserAlreadyExistsTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingUser\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RegisterCommand();

            Assert.AreEqual(result, "User already exists.");
        }

        [TestMethod]
        public void LoginCommandTestNotEnoughTokensTest()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string[] tokens = { "LOGIN", "" };
            string result = _commandService.LoginCommand(tokens);

            Assert.AreEqual(result, "Please use LOGIN [username] [password]");
        }

        [TestMethod]
        public void LoginCommandWrongUsernameTest()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string[] tokens = { "LOGIN", "yoosor", "pass1" };
            string result = _commandService.LoginCommand(tokens);

            Assert.AreEqual(result, "Invalid credentials.");
        }

        [TestMethod]
        public void LoginCommandWrongPasswordTest()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string[] tokens = { "LOGIN", "existingUser", "wrongPassword" };
            string result = _commandService.LoginCommand(tokens);

            Assert.AreEqual(result, "Invalid credentials.");
        }

        [TestMethod]
        public void LoginCommandTest()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string[] tokens = { "LOGIN", "2existingUser", "pass1" };

            try
            {
                string result = _commandService.LoginCommand(tokens);
            }

            catch (IOException ex)
            {
                // Catching Console.Clear exception
            }

            Assert.IsTrue(_userRepository.LoggedIn == "2existingUser");

            _userRepository.LogOut();
        }

        [TestMethod]
        public void AddMeetingCommandEmptyMeetingNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Meeting name can't be empty");
        }

        [TestMethod]
        public void AddMeetingCommandEmptyUserNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\n\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Responsible user must have a name");
        }

        [TestMethod]
        public void AddMeetingCommandNonExistantUserNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nNONexistant\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "User with username NONexistant does not exist.");
        }

        [TestMethod]
        public void AddMeetingCommandEmptyDescriptionTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nexistingUser\n\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Meeting description can't be empty");
        }

        [TestMethod]
        public void AddMeetingCommandInvalidCategoryTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nexistingUser\nb\nc\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Invalid meeting category");
        }

        [TestMethod]
        public void AddMeetingCommandInvalidMeetingTypeTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nexistingUser\nb\nShort\nqqq\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Invalid meeting type");
        }

        [TestMethod]
        public void AddMeetingCommandSuccessTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nexistingUser\nb\nShort\nLive\n\n\n\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.AddMeetingCommand();

            Assert.AreEqual(result, "Meeting added successfully.");

            _userRepository.Login("existingUser", "pass1");
            _meetingRepository.RemoveMeeting("a");
            _userRepository.LogOut();
        }

        [TestMethod]
        public void RemoveMeetingCommandSuccessTest()
        {
            var output = new StringWriter();
            var input = new StringReader("a\nexistingUser\nb\nShort\nLive\n\n\na\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            _commandService.AddMeetingCommand();

            _userRepository.Login("existingUser", "pass1");
            string result = _commandService.RemoveMeetingCommand();
            _userRepository.LogOut();

            Assert.AreEqual(result, "Meeting removed successfully.");
        }

        [TestMethod]
        public void RemoveMeetingCommandEmptyMeetingNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            string result = _commandService.RemoveMeetingCommand();

            Assert.AreEqual(result, "Meeting name may not be empty");
        }

        [TestMethod]
        public void RemoveMeetingCommandNotLoggedInTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingMeeting2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.RemoveMeetingCommand();

            Assert.AreEqual(result, "Only user: 2existingUser can remove this meeting.");
        }

        [TestMethod]
        public void AddAttendeeSuccessTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingMeeting2\n3existingUser1\n2000-05-05 12:30\n2000-05-05 16:30\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.AddAttendeeCommand();
            Assert.AreEqual(result, "Attendee added successfully.");

            _meetingRepository.RemoveAttendee("existingMeeting2", "3existingUser1");
        }

        [TestMethod]
        public void AddAttendeeEmptyMeetingNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n2existingUser2\n2000-05-05 12:30\n2000-05-05 16:30\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.AddAttendeeCommand();
            Assert.AreEqual(result, "Meeting name may not be empty");

        }

        [TestMethod]
        public void AddAttendeeEmptyAttendeeNameTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingMeeting2\n\n2000-05-05 12:30\n2000-05-05 16:30\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.AddAttendeeCommand();
            Assert.AreEqual(result, "Attendee name may not be empty");

        }

        [TestMethod]
        public void RemoveAttendeeSuccessTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingMeeting2\n2existingUser2\n2000-05-05 12:30\n2000-05-05 16:30\nexistingMeeting2\n2existingUser7\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            _commandService.AddAttendeeCommand();
            string result = _commandService.RemoveAttendeeCommand();
            Assert.AreEqual(result, "Attendee removal successful.");
        }

        [TestMethod]
        public void RemoveAttendeeMeetingNameEmptyTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n\n\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.RemoveAttendeeCommand();
            Assert.AreEqual(result, "Meeting name may not be empty");
        }

        [TestMethod]
        public void RemoveAttendeeAttendeeNameEmptyTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingMeeting2\n\n\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            string result = _commandService.RemoveAttendeeCommand();
            Assert.AreEqual(result, "Attendee name may not be empty");
        }

        [TestMethod]
        public void ListFilterTest()
        {
            // Setup
            User u = _userRepository.GetUser("existingUser");
            _meetingRepository.AddMeeting("MeetingToFilter", u, "filter", MeetingCategory.CodeMonkey, MeetingType.InPerson, DateTime.Parse("3000-01-01"), DateTime.Parse("4000-01-01"));
            _meetingRepository.AddAttendee("MeetingToFilter", "2existingUser3", DateTime.Parse("3000-01-01"), DateTime.Parse("4000-01-01"));
            _meetingRepository.AddAttendee("MeetingToFilter", "2existingUser4", DateTime.Parse("3000-01-01"), DateTime.Parse("4000-01-01"));
            _meetingRepository.AddAttendee("MeetingToFilter", "2existingUser5", DateTime.Parse("3000-01-01"), DateTime.Parse("4000-01-01"));
            _meetingRepository.AddAttendee("MeetingToFilter", "2existingUser6", DateTime.Parse("3000-01-01"), DateTime.Parse("4000-01-01"));

            var output = new StringWriter();
            var input = new StringReader("filter\nexistingUser\nCodeMonkey\nInPerson\n3000-01-01 00:00\n4000-01-01 00:00\n2\n5"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            var result = _commandService.FilterMeetings(_meetingRepository.GetMeetings(), "DESC RESP CAT TYPE DATEFROM DATETO ATTENDEESFROM ATTENDEESTO");

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.FirstOrDefault(x => x.MeetingName == "MeetingToFilter") != null);
        }
    }
}
