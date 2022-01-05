using Newtonsoft.Json;

namespace MeetingManagementApp
{
    [Serializable]
    public class UserMeeting
    {
        public string Name { get; private set; }
        public DateTime TimeFrom { get; private set; }
        public DateTime TimeTo { get; private set; }

        [JsonConstructor]
        public UserMeeting(string name, DateTime timeFrom, DateTime timeTo) {

            if(name == null || name == "") throw new ArgumentNullException("name");
            if (timeFrom > timeTo) throw new Exception("Invalid meeting time.");

            Name = name;
            TimeFrom = timeFrom;
            TimeTo = timeTo;
        }

    }
}
