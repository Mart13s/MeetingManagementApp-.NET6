using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingManagementApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeetingManagementAppTests.ReposTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private static IUserRepository _userRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) // Setup
        {
            if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");

            _userRepository = UserRepository.Instance;

            _userRepository.Register("existingUser", "pass1");
        }

        [ClassCleanup]
        public static void ClassCleanup() // Teardown
        {
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");
        }

        [TestMethod]
        public void UserRegisterNullUsernameTest()
        {
            bool result = _userRepository.Register(null, "password1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRegisterEmptyUsernameTest()
        {
            bool result = _userRepository.Register("", "password1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRegisterExistingUsernameTest()
        {
            bool result = _userRepository.Register("existingUser", "password1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRegisterNullPasswordTest()
        {
            bool result = _userRepository.Register("Username", null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRegisterEmptyPasswordTest()
        {
            bool result = _userRepository.Register("Username", "");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRegisterTest()
        {
            bool result = _userRepository.Register("Username", "pass1");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserLoginTest()
        {
            bool result = _userRepository.Login("existingUser", "pass1");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserLoginNullUsernameTest()
        {
            bool result = _userRepository.Login(null, "pass1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserLoginEmptyUsernameTest()
        {
            bool result = _userRepository.Login("", "pass1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserLoginNonExistantUsernameTest()
        {
            bool result = _userRepository.Login("notrealacc1337", "pass1");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserAddUserMeetingTest()
        {
            UserMeeting um = new UserMeeting("SomeAlternateMeeting", DateTime.MinValue, DateTime.MinValue);
            User u = _userRepository.GetUser("existingUser");
            _userRepository.AddUserMeeting("existingUser", um);

            Assert.IsTrue(u.Meetings.Contains(um));

            _userRepository.RemoveUserMeeting("existingUser", "SomeAlternateMeeting"); // Teardown
        }

        const string lowerBound = "1999-01-01";
        const string midBound = "2000-06-01";
        const string midBound2 = "2000-06-02";
        const string upperBound = "2002-01-01";


        [DataTestMethod]
        [DataRow(lowerBound, upperBound)]
        [DataRow(lowerBound, midBound)]
        [DataRow(midBound, upperBound)]
        public void UserAddUserMeetingBusyTest(string from, string to)
        {
            UserMeeting um = new UserMeeting("SomeMeeting", DateTime.Parse("2000-01-01"), DateTime.Parse("2001-01-01"));
            UserMeeting busyMeeting = new UserMeeting("BusyMeeting", DateTime.Parse(from), DateTime.Parse(to));

            User u = _userRepository.GetUser("existingUser");
            _userRepository.AddUserMeeting("existingUser", um);
            bool result = _userRepository.AddUserMeeting("existingUser", busyMeeting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRemoveUserMeetingTest()
        {
            UserMeeting um = new UserMeeting("QQ Meeting QQ", DateTime.Now, DateTime.Now);

            User u = _userRepository.GetUser("existingUser");
            bool result = _userRepository.AddUserMeeting("existingUser", um);

            Assert.IsTrue(result);

            result = _userRepository.RemoveUserMeeting("existingUser", "QQ Meeting QQ");

            Assert.IsFalse(u.Meetings.Contains(um));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid input arguments.")]
        public void UserRemoveUserMeetingNullUsernameTest()
        {

            bool result = _userRepository.RemoveUserMeeting(null, "QQ Meeting QQ");

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid input arguments.")]
        public void UserRemoveUserMeetingEmptyUsernameTest()
        {

            bool result = _userRepository.RemoveUserMeeting("", "QQ Meeting QQ");

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid input arguments.")]
        public void UserRemoveUserMeetingNullMeetingNameTest()
        {

            bool result = _userRepository.RemoveUserMeeting("existingUser", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid input arguments.")]
        public void UserRemoveUserMeetingEmptyMeetingNameTest()
        {

            bool result = _userRepository.RemoveUserMeeting("existingUser", "");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UserRemoveUserMeetingNonExistantMeetingTest()
        {

            bool result = _userRepository.RemoveUserMeeting("existingUser", "NON-EXISTANT");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UserRemoveUserMeetingNonExistantUserTest()
        {
            bool result = _userRepository.RemoveUserMeeting("NOTexistingUser", "MEETING_TO_ADD");
        }

        [TestMethod]
        public void UserRepositoryUserExistsTest()
        {
            bool result = _userRepository.UserExists("existingUser");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserRepositoryUserExistsUserNonExistantTest()
        {
            bool result = _userRepository.UserExists("NotExistantUser");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRepositoryUserExistsNullUsername()
        {
            bool result = _userRepository.UserExists(null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRepositoryUserExistsEmptyUsername()
        {
            bool result = _userRepository.UserExists("");
            Assert.IsFalse(result);
        }

        const string lowerBoundBusy = "999-01-01";
        const string midBoundBusy = "1000-06-01";
        const string midBound2Busy = "1000-06-02";
        const string upperBoundBusy = "1002-01-01";

        [DataTestMethod]
        [DataRow(lowerBoundBusy, upperBoundBusy)]
        [DataRow(lowerBoundBusy, midBoundBusy)]
        [DataRow(midBoundBusy, upperBoundBusy)]
        [DataRow(midBoundBusy, midBound2Busy)]
        public void UserRepositoryIsBusyTest(string from, string to)
        {
            UserMeeting um = new UserMeeting("TestMeetingRR", new DateTime(1000, 01, 01), new DateTime(1001, 01, 01));
            _userRepository.AddUserMeeting("existingUser", um);

            bool result = _userRepository.IsBusy("existingUser", DateTime.Parse(from), DateTime.Parse(to));

            Assert.IsTrue(result);

            _userRepository.RemoveUserMeeting("existingUser", "TestMeetingRR");
            

            
        }

        [TestMethod]
        public void UserRepositoryIsNotBusyTest()
        {

            bool result = _userRepository.IsBusy("existingUser", DateTime.Parse("2002-01-01"), DateTime.Parse("2002-01-01"));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRepositoryGetUserNonExistantTest()
        {
            Assert.IsNull(_userRepository.GetUser("EEEEEEEEEE"));
        }

        [TestMethod]
        public void UserRepositoryGetUserNullTest()
        {
            Assert.IsNull(_userRepository.GetUser(null));
        }

        [TestMethod]
        public void UserRepositoryGetUserEmptyTest()
        {
            Assert.IsNull(_userRepository.GetUser(null));
        }

        [TestMethod]
        public void UserRepositoryGetUserTest()
        {
            User u = _userRepository.GetUser("existingUser");
            Assert.IsNotNull(u);
            Assert.AreEqual(u.Username, "existingUser");
        }




    }
}
