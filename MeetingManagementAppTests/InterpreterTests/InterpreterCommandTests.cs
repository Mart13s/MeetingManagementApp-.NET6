using MeetingManagementApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MeetingManagementAppTests.InterpreterTests
{
    [TestClass]
    public class InterpreterCommandTests
    {
        private static Interpreter interpreter;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) // Setup
        {
            if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");

            IUserRepository userRepository = UserRepository.Instance;
            IMeetingRepository meetingRepository = MeetingRepository.Instance;

            userRepository.Register("existingUser", "pass1");

            interpreter = new Interpreter(new CommandService(userRepository, meetingRepository));
        }

        [TestMethod]
        public void RegisterUserTest()
        {
            var output = new StringWriter();
            var input = new StringReader("test\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(UserRepository.Instance.UserExists("test"));
        }

        [TestMethod]
        public void RegisterExistingUserTest()
        {
            var output = new StringWriter();
            var input = new StringReader("existingUser\npass1\npass1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("User already exists."));
        }

        [TestMethod]
        public void RegisterExistingIncorrectPasswordFormat()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser\npass!!!!!1\npass!!!!!1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Password must contain a number and a letter"));
        }

        [TestMethod]
        public void RegisterPasswordTooShort()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser\np1\np1\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Password must contain atleast 3 characters"));
        }

        [TestMethod]
        public void RegisterPasswordsDontMatch()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser\nsomething1\njglDiff2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Passwords don't match"));
        }

        [TestMethod]
        public void RegisterPasswordEmpty()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser\n\njglDiff2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Password may not be empty!"));
        }

        [TestMethod]
        public void RegisterIncorrectUsernameFormat()
        {
            var output = new StringWriter();
            var input = new StringReader("newUser77\njglDiff2\njglDiff2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Username may only contain letters!"));
        }

        [TestMethod]
        public void RegisterIncorrectUsernameEmpty()
        {
            var output = new StringWriter();
            var input = new StringReader("\njglDiff2\njglDiff2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Username may not be empty!"));
        }

        [TestMethod]
        public void RegisterIncorrectUsernameTooLong()
        {
            var output = new StringWriter();
            var input = new StringReader("jglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiffjglDiff\n" +
                "jglDiff2\njglDiff2\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REGISTER");
            Assert.IsTrue(output.ToString().Contains("Username may not be longer than 30 characters!"));
        }

        [TestMethod]
        public void LoginWrongUser()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = false;

            try
            {
                result = interpreter.Interpret("LOGIN qwe pass1");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoginWrongPassword()
        {

            var output = new StringWriter();
            Console.SetOut(output);

            try
            {
                interpreter.Interpret("LOGIN existingUser wrongPassword1");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }

            Assert.IsTrue(UserRepository.Instance.LoggedIn == "");

        }

        [TestMethod]
        public void LoginNoUserNoPassword()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("LOGIN");
            Assert.IsTrue(output.ToString().Contains("Please use LOGIN [username] [password]"));
        }

        [TestMethod]
        public void LoginSuccess()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            try
            {
                interpreter.Interpret("LOGIN existingUser pass1");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }

            Assert.IsTrue(UserRepository.Instance.LoggedIn == "existingUser");

            try
            {
                interpreter.Interpret("LOGOUT");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }
        }

        [TestMethod]
        public void LogoutTest()
        {
            try
            {
                interpreter.Interpret("LOGIN existingUser pass1");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }

            Assert.IsTrue(UserRepository.Instance.LoggedIn == "existingUser");

            try
            {
                interpreter.Interpret("LOGOUT");
            }
            catch
            {
                // Console.Clear() throws exception, because console is mocked
            }

            Assert.IsTrue(UserRepository.Instance.LoggedIn == "");

        }

        [TestMethod]
        public void ListTest()
        {
            var output = new StringWriter();
            var input = new StringReader(""); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = false;

            try
            {
                result = interpreter.Interpret("LIST");
            }

            catch (IOException e)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddMeetingTest() 
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("ADD_MEETING");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddAttendeeTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("ADD_ATTENDEE");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveMeetingTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REMOVE_MEETING");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveAttendeeTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("REMOVE_ATTENDEE");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ClearTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);


            bool result = false;

            try
            {
                result = interpreter.Interpret("CLEAR");
            }

            catch
            {
                // Catching Console handle exception. (Not a console app)
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ClrTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = false;

            try
            {
                result = interpreter.Interpret("CLR");
            }

            catch
            {
                // Catching Console handle exception. (Not a console app)
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExitTest()
        {
            var output = new StringWriter();
            var input = new StringReader("\n"); // Mocking user inputs.
            Console.SetOut(output);
            Console.SetIn(input);

            bool result = interpreter.Interpret("EXIT");
            Assert.IsFalse(result);
        }

        [ClassCleanup]
        public static void ClassCleanup() // Teardown
        {
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");
        }
    }
}
