using Newtonsoft.Json;

namespace MeetingManagementApp;

public interface IMeetingRepository
{
    void AddAttendee(string meetingName, string attendeeName, DateTime from, DateTime to);
    void AddMeeting(string name, User responsible, string description, MeetingCategory category, MeetingType meetType, DateTime from, DateTime to);
    ICollection<Meeting> GetMeetings();
    bool MeetingExists(string name);
    void RemoveAttendee(string meetingName, string attendeeName);
    void RemoveMeeting(string meetingName);
}

public class MeetingRepository : IMeetingRepository
{
    private const string MEETING_DATA_URL = "../../../Data/MeetingData.json";

    // Using eager instantiation to make sure we have read json data, before any commands are called.
    private static MeetingRepository _instance = new MeetingRepository();

    private UserRepository _userRepository = UserRepository.Instance;
    private List<Meeting> _meetings = new List<Meeting>();

    public MeetingRepository()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        if (File.Exists(MEETING_DATA_URL))
        {
            var result = ReadData();
            if (result != null) _meetings = result;
        }

        else File.Create(MEETING_DATA_URL).Close();


    }

    public static MeetingRepository Instance
    {
        get { return _instance; }
    }

    public void AddMeeting(string name, User responsible, string description,
        MeetingCategory category, MeetingType meetType, DateTime from, DateTime to)
    {
        if (name == null || name == "" 
            || responsible == null || responsible.Username == null  || responsible.Username == "" 
            || description == null || description == "")
            throw new Exception("Invalid meeting arguments.");

        if (_userRepository.UserExists(responsible.Username) == false)
            throw new Exception("User responsible doesn't exist");

        if (MeetingExists(name))
            throw new Exception("Meeting with that name already exists");

        if (from > to)
            throw new Exception("Invalid meeting dates");

        if (_userRepository.IsBusy(responsible.Username, from, to))
            throw new Exception("User is busy with other meeting");


        Meeting m = new Meeting(name, responsible.Username, description, category, meetType, from, to);

        // Resuing UserMeeting class for atendees
        m.AddAttendee(new UserMeeting(responsible.Username, from, to));

        // Updating user meeting log
        _userRepository.AddUserMeeting(responsible.Username, new UserMeeting(name, from, to));

        // Adding meeting
        _meetings.Add(m);

        SaveChanges();


    }

    /// <summary>
    /// Meeting removal method throws errors, if meeting is non existant or user is unauthorized to remove it
    /// </summary>
    /// <param name="meetingName">Name of the meeting to be removed.</param>
    /// <exception cref="ArgumentException">Thrown, if meeting of input name doesn't exist</exception>
    /// <exception cref="Exception">Thrown, if user is unauthorized to remove the meeting.</exception>
    public void RemoveMeeting(string meetingName)
    {
        if (MeetingExists(meetingName) == false)
            throw new ArgumentException($"Meeting with name {meetingName} does not exist.");

        var meeting = _meetings.FirstOrDefault(q => string.Compare(q.MeetingName, meetingName) == 0);

        if (string.Compare(_userRepository.LoggedIn, meeting.ResponsiblePerson) != 0)
            throw new Exception($"Only user: {meeting.ResponsiblePerson} can remove this meeting.");

        // Removing meeting from atendee lists
        foreach (UserMeeting um in meeting.Attendees)
        {
            var usr = _userRepository.GetUser(um.Name);
            _userRepository.RemoveUserMeeting(usr.Username, meetingName);
        }

        // Removing meeting from meeting list
        _meetings.Remove(meeting);

        SaveChanges();

    }


    /// <summary>
    /// This method marshalls all meeting data to file with URL "MEETING_DATA_URL"
    /// </summary>
    private void SaveChanges()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        if (File.Exists(MEETING_DATA_URL)) File.Delete(MEETING_DATA_URL);
        File.WriteAllText(MEETING_DATA_URL, JsonConvert.SerializeObject(_meetings));

    }

    /// <summary>
    /// This method unmarshalls meeting data from json file.
    /// </summary>
    /// <returns>A list of meetings unmarshalled from json.</returns>
    private List<Meeting> ReadData()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        try
        {
            if (File.Exists(MEETING_DATA_URL))
            {
                string json = File.ReadAllText(MEETING_DATA_URL);
                return JsonConvert.DeserializeObject<List<Meeting>>(json);
            }
        }

        catch { }

        return new List<Meeting>();

    }

    public bool MeetingExists(string name)
    {
        return _meetings.FirstOrDefault(x => x.MeetingName == name) != null;
    }

    public ICollection<Meeting> GetMeetings()
    {
        return _meetings.AsReadOnly(); // Returning as read only, to maintain encapsulation
    }

    public void AddAttendee(string meetingName, string attendeeName, DateTime from, DateTime to)
    {
        if (meetingName == null)
            throw new ArgumentNullException("Invalid meeting name.");

        if (attendeeName == null)
            throw new ArgumentNullException("Invalid attendee name.");

        var meeting = _meetings.FirstOrDefault(q => string.Compare(q.MeetingName, meetingName) == 0);
        if (meeting == null)
            throw new Exception($"Meeting with name {meetingName} not found.");

        var attendee = _userRepository.GetUser(attendeeName);
        if (attendeeName == null)
            throw new Exception($"User with name {attendeeName} not found.");

        if (meeting.Attendees.FirstOrDefault(q => string.Compare(q.Name, attendeeName) == 0) != null)
            throw new Exception($"User: {attendeeName} is already in this meeting");

        if (from < meeting.From || to > meeting.To)
            throw new Exception("Invalid user meeting time.");

        if (_userRepository.IsBusy(attendeeName, from, to))
            throw new Exception($"User {attendeeName} is busy during that time.");

        _userRepository.AddUserMeeting(attendeeName, new UserMeeting(meetingName, from, to));
        meeting.AddAttendee(new UserMeeting(attendeeName, from, to));

        SaveChanges();


    }

    public void RemoveAttendee(string meetingName, string attendeeName)
    {

        var meeting = _meetings.FirstOrDefault(q => string.Compare(q.MeetingName, meetingName) == 0);
        if (meeting == null)
            throw new Exception($"Meeting: {meetingName} does not exist.");

        var user = _userRepository.GetUser(attendeeName);
        if (user == null)
            throw new Exception($"User: {attendeeName} does not exist");

        if (string.Compare(meeting.ResponsiblePerson, user.Username) == 0)
            throw new Exception("Cannot remove user responsible for the meeting.");

        var attendee = meeting.Attendees.FirstOrDefault(q => string.Compare(q.Name, attendeeName) == 0);
        if (attendee == null)
            throw new Exception($"User: {attendeeName} is not attending meeting {meetingName}");

        meeting.RemoveAttendee(attendee);
        _userRepository.RemoveUserMeeting(attendeeName, meetingName);

        SaveChanges();

    }
}
