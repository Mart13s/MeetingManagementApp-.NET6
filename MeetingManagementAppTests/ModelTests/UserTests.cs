using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeetingManagementApp;
using System;

namespace MeetingManagementAppTests.ModelTests
{

    [TestClass]
    public class UserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            " Invalid arguments while creating User object")]
        public void UserNullUsernameTest()
        {
            User u = new User(null, "hash", "salt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
    " Invalid arguments while creating User object")]
        public void UserEmptyUsernameTest()
        {
            User u = new User("", "hash", "salt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
" Invalid arguments while creating User object")]
        public void UserNullHashTest()
        {
            User u = new User("username", null, "salt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
" Invalid arguments while creating User object")]
        public void UserEmptyHashTest()
        {
            User u = new User("username", "", "salt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
" Invalid arguments while creating User object")]
        public void UserNullSaltTest()
        {
            User u = new User("username", "hash", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
" Invalid arguments while creating User object")]
        public void UserEmptySaltTest()
        {
            User u = new User("username", "hash", "");
        }

        [TestMethod]
        public void UserResetPassTest()
        {
            User u = new User("username", "hash", "salt");

            u.ResetPassSaltHash("newHash", "newSalt");

            Assert.AreEqual(u.Hash, "newHash");
            Assert.AreEqual(u.Salt, "newSalt");
        }

        [TestMethod]
        public void UserAddNullMeetingTest()
        {
            User u = new User("username", "hash", "salt");

            bool result = u.AddUserMeeting(null);
           

            Assert.IsFalse(result); 
        }

        [TestMethod]
        public void UserAddMeetingTest()
        {
            User u = new User("username", "hash", "salt");
            UserMeeting um = new UserMeeting("Meeting", DateTime.Now, DateTime.Now);

            bool result = u.AddUserMeeting(um);


            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserRemoveNullMeetingTest()
        {
            User u = new User("username", "hash", "salt");
            UserMeeting um = new UserMeeting("Meeting", DateTime.Now, DateTime.Now);

            bool result = u.RemoveMeeting(null);


            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRemoveNonParticipatedMeetingTest()
        {
            User u = new User("username", "hash", "salt");
            UserMeeting um = new UserMeeting("Meeting", DateTime.Now, DateTime.Now);
            UserMeeting otherMeeting = new UserMeeting("SomeOtherMeeting", DateTime.Now, DateTime.Now);

            u.AddUserMeeting(um);

            bool result = u.RemoveMeeting(otherMeeting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRemoveMeetingTest()
        {
            User u = new User("username", "hash", "salt");
            UserMeeting um = new UserMeeting("Meeting", DateTime.Now, DateTime.Now);

            u.AddUserMeeting(um);

            bool result = u.RemoveMeeting(um);

            Assert.IsTrue(result);
        }
    }
}
