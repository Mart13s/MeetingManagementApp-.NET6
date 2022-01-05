using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;

namespace MeetingManagementApp;

public interface IUserRepository
{
    string LoggedIn { get; }

    bool AddUserMeeting(string username, UserMeeting userMeeting);
    User GetUser(string username);
    bool IsBusy(string username, DateTime from, DateTime to);
    bool Login(string username, string password);
    void LogOut();
    bool Register(string username, string password);
    bool RemoveUserMeeting(string username, string meetingName);
    bool UserExists(string username);
}

public class UserRepository : IUserRepository
{

    private const string USER_DATA_URL = "../../../Data/UserData.json";

    // Using eager instantiation to make sure we have read json data, before any commands are called.
    private static UserRepository _instance = new UserRepository();

    private List<User> _users = new List<User>();
    private string _currentUser = "";

    public UserRepository()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        if (File.Exists(USER_DATA_URL))
        {
            var result = ReadData();
            if (result != null) _users = result;
        }

        else File.Create(USER_DATA_URL).Close();


    }

    public static UserRepository Instance
    {
        get
        {
            return _instance;
        }
    }

    public string LoggedIn
    {
        get { return _currentUser; }
    }


    /// <summary>
    /// If a unique username is specified, this method
    /// generates a new salt and hash (using HMACSHA256) for the user,
    /// and then adds that user to the list.
    /// </summary>
    /// <param name="username">Unique username string.</param>
    /// <param name="password">Non-null password field.</param>
    /// <returns>True, if registration is successful. False, if failed.</returns>
    public bool Register(string username, string password)
    {

        if (username == null || username == "" || password == null || password == "") return false;

        var user = _users.FirstOrDefault(q => string.Equals(username, q.Username));
        if (user != null) return false;


        // Generating Salt
        byte[] salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        string saltString = Convert.ToBase64String(salt);

        // Generating Hash with HMACSHA256 bit
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32));

        user = new User(username, hash, saltString);

        _users.Add(user);

        SaveChanges();
        return true;
    }


    /// <summary>
    /// This method logs a user in, if a credentials check is passed.
    /// </summary>
    /// <returns>True, if registration is successful. False, if failed.</returns>
    public bool Login(string username, string password)
    {

        if (username == null || username == "" || password == null || password == "") return false;

        var user = _users.FirstOrDefault(q => string.Equals(username, q.Username));
        if (user == null) return false;


        byte[] salt = Convert.FromBase64String(user.Salt);

        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32));


        bool equals = true;
        string userHash = user.Hash;

        // Safe hash check
        for (int i = 0; i < userHash.Length; i++)
        {
            if (userHash[i] != hash[i]) equals = false;
        }

        if (equals) _currentUser = username;

        return equals;
    }

    public void LogOut()
    {
        _currentUser = "";
    }

    private void SaveChanges()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        if (File.Exists(USER_DATA_URL)) File.Delete(USER_DATA_URL);
        File.WriteAllText(USER_DATA_URL, JsonConvert.SerializeObject(_users));

    }

    private List<User> ReadData()
    {
        if (Directory.Exists("../../../Data") == false) Directory.CreateDirectory("../../../Data");

        try
        {
            if (File.Exists(USER_DATA_URL))
            {
                string json = File.ReadAllText(USER_DATA_URL);
                return JsonConvert.DeserializeObject<List<User>>(json);
            }
        }

        catch { }

        return new List<User>();

    }

    public bool AddUserMeeting(string username, UserMeeting userMeeting)
    {
        if (username == "" || username == null || userMeeting == null) return false;
        User user = _users.FirstOrDefault(x => x.Username == username);

        if (user == null) return false;

        if (IsBusy(user.Username, userMeeting.TimeFrom, userMeeting.TimeTo)) return false;

        if (userMeeting.TimeFrom > userMeeting.TimeTo) return false;

        user.AddUserMeeting(userMeeting);

        SaveChanges();
        return true;
    }

    public bool RemoveUserMeeting(string username, string meetingName)
    {
        if (username == "" || username == null || meetingName == "" || meetingName == null)
            throw new ArgumentException("Invalid input arguments.");

        User user = _users.FirstOrDefault(x => x.Username == username);
        if (user == null)
            throw new Exception($"User with name {username} not found");

        UserMeeting um = user.Meetings.FirstOrDefault(x => x.Name.Contains(meetingName));
        if (um == null) throw new Exception($"Meeting with name {meetingName} not found");
        else user.RemoveMeeting(um);

        SaveChanges();
        return true;
    }

    public bool UserExists(string username)
    {
        if (username == null || username == "") return false;

        return _users.FirstOrDefault(x => x.Username == username) != null;
    }

    /// <summary>
    /// This method checks, if a user is busy during meeting time specified in from and to params
    /// </summary>
    /// <returns>True, if busy. False, if not.</returns>
    public bool IsBusy(string username, DateTime from, DateTime to)
    {
        var user = _users.FirstOrDefault(q => q.Username == username);
        if (user == null) return true;

        foreach (var userMeeting in user.Meetings)
        {

            //      If meeting started before mine, but ended after specified meeting had started
            // OR   If meeting that ends after mine, but starts before specified meeting ends
            // OR   If meeting happens during the same time
            // OR   If specified meeting happens during another meeting
            if ((userMeeting.TimeFrom < from && userMeeting.TimeTo > from) ||
                 (userMeeting.TimeTo > to && userMeeting.TimeFrom < to)    ||
                 (userMeeting.TimeFrom >= from && userMeeting.TimeTo <= to) ||
                 (userMeeting.TimeFrom <= from && userMeeting.TimeTo >= to)
               )
            {
                return true; // User is busy at that time.
            }
        }

        return false;
    }

    public User GetUser(string username)
    {
        User user = _users.FirstOrDefault(q => q.Username == username);
        return user;
    }

}
