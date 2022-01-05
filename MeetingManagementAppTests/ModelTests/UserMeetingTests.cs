using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeetingManagementApp;

namespace MeetingManagementAppTests.ModelTests
{
    [TestClass]
    public class UserMeetingTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "name")]
        public void UserMeetingNullMeetingName()
        {
            UserMeeting um = new UserMeeting(null, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
    "name")]
        public void UserMeetingEmptyMeetingName()
        {
            UserMeeting um = new UserMeeting("", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
"Invalid meeting time.")]
        public void UserMeetingInvalidMeetingTime()
        {
            UserMeeting um = new UserMeeting("MeetingM", DateTime.Now.AddDays(1), DateTime.Now);
        }

    }
}
