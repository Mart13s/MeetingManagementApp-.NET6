using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingManagementApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeetingManagementAppTests.ReposTests
{
    [TestClass]
    public class MeetingRepositoryTests
    {
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

            _userRepository.Register("existingUser", "pass1");
            _userRepository.Register("existingUser2", "pass1");
            _userRepository.Register("existingUser3", "pass1");
            _userRepository.Register("existingUser4", "pass1");
            _userRepository.Register("existingUser5", "pass1");
            _userRepository.Register("existingUser6", "pass1");
            _userRepository.Register("existingUser7", "pass1");
            _userRepository.Register("existingUser8", "pass1");
            _userRepository.Register("existingUser9", "pass1");
            _userRepository.Register("Busy", "pass1");
            _meetingRepository.AddMeeting("reallyBusyMeeting", _userRepository.GetUser("Busy"), "desc",
                                          MeetingCategory.Hub, MeetingType.Live, DateTime.MinValue, DateTime.MaxValue);

            User u = _userRepository.GetUser("existingUser");
            _meetingRepository.AddMeeting("existingMeeting", u, "Desc", MeetingCategory.CodeMonkey, MeetingType.InPerson, DateTime.Parse("2000-01-01"), DateTime.Parse("2001-01-01"));
        }

        [ClassCleanup]
        public static void ClassCleanup() // Teardown
        {
            if (File.Exists("../../../Data/MeetingData.json")) File.Delete("../../../Data/MeetingData.json");
            if (File.Exists("../../../Data/UserData.json")) File.Delete("../../../Data/UserData.json");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid meeting arguments.")]
        public void MeetingReposAddMeetingNullNameTest()
        {
            _meetingRepository.AddMeeting(null, _userRepository.GetUser("existingUser"),
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid meeting arguments.")]
        public void MeetingReposAddMeetingEmptyNameTest()
        {
            _meetingRepository.AddMeeting("", _userRepository.GetUser("existingUser"),
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid meeting arguments.")]
        public void MeetingReposAddMeetingNullResponsible()
        {
            
            _meetingRepository.AddMeeting("name", null,
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User responsible doesn't exist")]
        public void MeetingReposAddMeetingUserNonExistantTest()
        {

            User u = new User("non_existant", "hash", "salt");
            _meetingRepository.AddMeeting("name", u,
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Meeting with that name already exists")]
        public void MeetingReposAddMeetingExistsAlreadyTest()
        {

            _meetingRepository.AddMeeting("existingMeeting", _userRepository.GetUser("existingUser"),
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid meeting dates")]
        public void MeetingReposAddMeetingInvalidDatesTest()
        {

            _meetingRepository.AddMeeting("qeqeweqwe", _userRepository.GetUser("existingUser"),
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now.AddYears(1), DateTime.Now);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User is busy with other meeting")]
        public void MeetingReposAddMeetingUserBusyTest()
        {
            _userRepository.Register("AlwaysBusy", "pass1");
            _userRepository.AddUserMeeting("AlwaysBusy", new UserMeeting("BusyMeeting", DateTime.MinValue, DateTime.MaxValue));

            _meetingRepository.AddMeeting("qeqweqweqwe", _userRepository.GetUser("AlwaysBusy"),
                "desc", MeetingCategory.Short, MeetingType.Live, DateTime.Now, DateTime.Now);

        }

        [TestMethod]
        public void MeetingReposAddMeetingSuccessTest()
        {
            User u = _userRepository.GetUser("existingUser9");
            _meetingRepository.AddMeeting("userMeeting9", u, "desc9", MeetingCategory.TeamBuilding, MeetingType.Live, DateTime.Now, DateTime.Now);

            Assert.IsTrue(_meetingRepository.MeetingExists("userMeeting9"));
            Assert.IsTrue(u.Meetings.FirstOrDefault(x => x.Name == "userMeeting9") != null);

            _userRepository.Login("existingUser9", "pass1");
            _meetingRepository.RemoveMeeting("userMeeting9");
            _userRepository.LogOut();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MeetingReposRemoveMeetingNonExistantTest()
        {
            _meetingRepository.RemoveMeeting("nonexistantmeeting");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposRemoveMeetingNotLoggedInTest()
        {
            _meetingRepository.RemoveMeeting("existingMeeting");
        }

        [TestMethod]
        public void MeetingReposRemoveMeetingSuccessTest()
        {
            User u = _userRepository.GetUser("existingUser8");
            _meetingRepository.AddMeeting("existingMeeting8", u, "desc", MeetingCategory.CodeMonkey, MeetingType.Live, DateTime.Now, DateTime.Now);

            bool result = _meetingRepository.MeetingExists("existingMeeting8");
            _userRepository.Login("existingUser8", "pass1");

            Assert.IsTrue(result);
            _meetingRepository.RemoveMeeting("existingMeeting8");

            result = _meetingRepository.MeetingExists("existingMeeting8");
            Assert.IsFalse(result);

            _userRepository.LogOut();
        }

        [TestMethod]
        public void MeetingReposMeetingExistsFailTest()
        {
            bool result = _meetingRepository.MeetingExists("nonexistantmeeting");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MeetingReposMeetingExistsSuccessTest()
        {
            bool result = _meetingRepository.MeetingExists("existingMeeting");

            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MeetingReposAddAttendeeMeetingNameNullTest()
        {
            _meetingRepository.AddAttendee(null, "existingUser", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MeetingReposAddAttendeeAttendeeNameNullTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", null, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposAddAttendeeNotExistingMeetingTest()
        {
            _meetingRepository.AddAttendee("notExistingMeeting", "existingUser", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposAddAttendeeNotExistingUserTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "notExistingUser", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposAddAttendeeAlreadyInMeetingTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "existingUser", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposAddAttendeeInvalidTimeTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "existingUser2", DateTime.Now.AddDays(1), DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MeetingReposAddAttendeeIsBusyTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "Busy", DateTime.Parse("2000-01-01"), DateTime.Parse("2000-01-01"));
        }

        [TestMethod]
        public void MeetingReposAddAttendeeSuccessTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "existingUser7", DateTime.Parse("2000-01-01"), DateTime.Parse("2000-01-01"));
            
            UserMeeting result = _userRepository.GetUser("existingUser7")
                .Meetings.FirstOrDefault(x => x.Name == "existingMeeting");

            Assert.AreNotEqual(result, default(UserMeeting));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Meeting: does not exist.")]
        public void MeetingReposRemoveAttendeeMeetingNonExistantTest()
        {
            _meetingRepository.RemoveAttendee(null, "existingUser");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User: notExistingUser does not exist")]
        public void MeetingReposRemoveAttendeeUserNotExistingTest()
        {
            _meetingRepository.RemoveAttendee("existingMeeting", "notExistingUser");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Cannot remove user responsible for the meeting.")]
        public void MeetingReposRemoveAttendeeUserResponsibleTest()
        {
            _meetingRepository.RemoveAttendee("existingMeeting", "existingUser");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User: existingUser4 is not attending meeting existingMeeting")]
        public void MeetingReposRemoveAttendeeNotAttendingUserTest()
        {
            _meetingRepository.RemoveAttendee("existingMeeting", "existingUser4");
        }

        [TestMethod]
        public void MeetingReposRemoveAttandeeSuccessTest()
        {
            _meetingRepository.AddAttendee("existingMeeting", "existingUser5", DateTime.Parse("2000-01-01"), DateTime.Parse("2000-01-01"));
            User u = _userRepository.GetUser("existingUser5");

            Assert.IsNotNull(u.Meetings.FirstOrDefault(x => x.Name == "existingMeeting"));

            _meetingRepository.RemoveAttendee("existingMeeting", "existingUser5");

            Assert.IsNull(u.Meetings.FirstOrDefault(x => x.Name == "existingMeeting"));
        }
    }
}
