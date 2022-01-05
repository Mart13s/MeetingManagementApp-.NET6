namespace MeetingManagementApp;

[Serializable]
public class User {

    public string Username { get; private set; }
    public string Hash { get; private set; }
    public string Salt { get; private set; }

    public List<UserMeeting> Meetings { get; private set; }

    public User(string username, string hash, string salt)
    {
        if (username == "" || username == null
        ||  hash == ""     || hash == null 
        ||  salt == ""     || salt == null) 
            throw new ArgumentException(" Invalid arguments while creating User object");

        Username = username;
        Hash = hash;
        Salt = salt;
        Meetings = new List<UserMeeting>();
    }
    public void ResetPassSaltHash(string hash, string salt)
    {
        if (hash == null || hash == "" || salt == null || salt == "") return;

        Hash = hash;
        Salt = salt;
        
    }

    public bool AddUserMeeting(UserMeeting meeting)
    {
        if (meeting == null) return false;

        Meetings.Add(meeting);
        return true;
    }

    public bool RemoveMeeting(UserMeeting meeting)
    {
        if(meeting == null || meeting.Name == null || meeting.Name == "") return false;

        var meetingToRemove = Meetings.FirstOrDefault(q => meeting.Name == q.Name);
        if (meetingToRemove == null) return false;

        Meetings.Remove(meetingToRemove);
        return true;
    }
}
