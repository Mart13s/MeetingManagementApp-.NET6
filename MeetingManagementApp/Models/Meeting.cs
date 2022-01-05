using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MeetingManagementApp;

[JsonConverter(typeof(StringEnumConverter))]
public enum MeetingCategory
{
    [EnumMember(Value = "CodeMonkey")] // Adding EnumMember properties, to fix enum deserializing
    CodeMonkey,
    [EnumMember(Value = "Hub")]
    Hub,
    [EnumMember(Value = "Short")]
    Short,
    [EnumMember(Value = "TeamBuilding")]
    TeamBuilding
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MeetingType
{
    [EnumMember(Value = "Live")]
    Live,

    [EnumMember(Value = "InPerson")]
    InPerson
}

[Serializable]
public class Meeting
{

    public string MeetingName { get; private set; }
    public string ResponsiblePerson { get; private set; }
    public string Description { get; private set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public MeetingCategory Category { get; private set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public MeetingType MeetType { get; private set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }

    [JsonProperty("Attendees")]
    public List<UserMeeting> Attendees { get; private set; }

    public Meeting(string meetingName, string responsiblePerson, string description,
        MeetingCategory category, MeetingType type, DateTime from, DateTime to)
    {
        Attendees = new List<UserMeeting>();

        if (meetingName == null || meetingName == "") throw new ArgumentNullException("name");
        if (responsiblePerson == null || responsiblePerson == "") throw new Exception(" Meeting must have a responsible person");
        if (description == null || description == "") throw new ArgumentNullException("description");
        if (from > to) throw new Exception(" Invalid meeting time");

        MeetingName = meetingName;
        ResponsiblePerson = responsiblePerson;
        Description = description;
        From = from;
        To = to;
        Category = category;
        MeetType = type;
        
    }

    public void AddAttendee(UserMeeting um)
    {
        Attendees.Add(um);
    }
    public void RemoveAttendee(UserMeeting um)
    {
        Attendees.Remove(um);
    }






}
