using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeetingManagementApp;

namespace MeetingManagementAppTests.ModelTests
{
    [TestClass]
    public class MeetingTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "name")]
        public void MeetingNullMeetingNameTest()
        {
            Meeting m = new Meeting(null, "responsible", "desc", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
    "name")]
        public void MeetingEmptyMeetingNameTest()
        {
            Meeting m = new Meeting("", "responsible", "desc", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), " Meeting must have a responsible person")]
        public void MeetingResponsibleNullTest()
        {
            Meeting m = new Meeting("SomeName", null, "desc", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), " Meeting must have a responsible person")]
        public void MeetingResponsibleEmptyTest()
        {
            Meeting m = new Meeting("SomeName", "", "desc", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "description")]
        public void MeetingDescriptionNullTest()
        {
            Meeting m = new Meeting("SomeName", "responsible", null, MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "description")]
        public void MeetingDescriptionEmptyTest()
        {
            Meeting m = new Meeting("SomeName", "responsible", "", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), " Invalid meeting time")]
        public void MeetingInvalidTimeTest()
        {
            Meeting m = new Meeting("SomeName", "responsible", "description", MeetingCategory.Hub, MeetingType.Live, DateTime.Now.AddDays(1), DateTime.Now);
        }

        [TestMethod]
        public void MeetingAddAttendee()
        {
            UserMeeting um = new UserMeeting("test", DateTime.Now, DateTime.Now);
            Meeting m = new Meeting("SomeName", "responsible", "description", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);

            m.AddAttendee(um);

            bool result = m.Attendees.Contains(um);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MeetingRemoveAttendee()
        {
            UserMeeting um = new UserMeeting("test", DateTime.Now, DateTime.Now);
            Meeting m = new Meeting("SomeName", "responsible", "description", MeetingCategory.Hub, MeetingType.Live, DateTime.Now, DateTime.Now);

            m.AddAttendee(um);

            bool result1 = m.Attendees.Contains(um);

            m.RemoveAttendee(um);

            bool result2 = m.Attendees.Contains(um);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }


    }
}
