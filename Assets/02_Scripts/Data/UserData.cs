using Firebase.Database;

[System.Serializable]
public class UserData : ILoadableFromSnapshot
{
    public string uid;
    public string name;
    public string email;
    
    public UserData()
    {
        this.uid = "uid";
        this.name = "name";
        this.email = "email";
    }

    public UserData(string uid, string name, string email)
    {
        this.uid = uid;
        this.name = name;
        this.email = email;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        email = snap.Child("email").Value.ToString();
        name = snap.Child("name").Value.ToString();
        uid = snap.Child("uid").Value.ToString();
    }

}