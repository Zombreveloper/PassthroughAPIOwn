/* This class manages the UserDataCVD. This exists because PlayerPrefs doesn't provide support for lists... thanks for nothing.
 * Saves the data to a JSON File and retrieves it from there. Important to keep data between sessions.
 * Key for retrieving data will be the name set in UserDataCVD
 * 
 * Supposed to exist between Scene changes to prevent Data Loss. So Singleton and DontDestroyOnLoad is incentivised.
 * Not sure if that really is necessary but better be safe.
 */

using System.IO;
using Oculus.Platform.Models;
using ProfileSaveEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string filePath;

    public AllUserData Data = new AllUserData(); //Note: Maybe I don't need to make a new one everytime?
    public UserDataCVD currentUser = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "userdataCVD.json");
            InitializeData();
            SetUser();
        }
        else Destroy(gameObject);

    }

    private void SetUser()
    {
        var profile = Data.Users[0];
        if (profile == null)
        {
            Debug.Log("No Profile could be loaded.");
        }
        else
        {
            //Hole einfach irgendein Profil ran. (Ich will mich nicht darum kümmern, das letzte Profil über Sessions hinaus zu speichern)
            currentUser = profile;
        }
    }

    public void AddUser(UserDataCVD user)
    {
        //to distinguish between names that are already set.
        //Overwrites Entry with same name instead of adding it because I wouldn't know how I would search by name otherwise
        //(Code provided by ChatGPT)
        int index = Data.Users.FindIndex(u => u.Name == user.Name);
        if (index >= 0)
        {
            Data.Users[index] = user;
        }
        else
        {
            Data.Users.Add(user);
        }
        SaveData();
        ProfileSaveEventManager.Save.OnProfileCreated.Invoke(user);
    }

    //Hier neue Abfrage rein: ist überhaupt ein Name im String?
    //(Sonst Nullpointer wenn keine Profile existent und Delete wird gedrückt)
    public void DeleteUser(string name)
    {
        int index = Data.Users.FindIndex(u => u.Name == name);
        if (index >= 0)
        {
            Data.Users.RemoveAt(index);
            SaveData();
            ProfileSaveEventManager.Save.OnProfileDeleted.Invoke(name);
        }
        else
        {
            Debug.LogWarning("Profile not found in JSON file. Something went seriously wrong...");
        }
    }

    public UserDataCVD GetByName(string name)
    {
        //return 
        var u = Data.Users.Find(u => u.Name == name);
        currentUser = u;
        return u;
    }

    public void SaveData()
    {
        string jsonForm = JsonUtility.ToJson(Data, true);
        File.WriteAllText(filePath, jsonForm);
    }

    public void InitializeData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Data = JsonUtility.FromJson<AllUserData>(json);
        }
        else
        {
            Data = new AllUserData();
        }
    }

    public UserDataCVD FallbackProfile()
    {
        var fallbackuser = new UserDataCVD();

        fallbackuser.Name = "FallbackProfile";
        fallbackuser.ProtanScore = 120f;
        fallbackuser.DeutanScore = 120f;
        fallbackuser.TritanScore = 120f;
        fallbackuser.ProtanUV = 1f;
        fallbackuser.DeutanUV = 1f;
        fallbackuser.TritanUV = 1f;

        return fallbackuser;
    }
}

namespace ProfileSaveEvent
{
    public static class ProfileSaveEventManager
    {
        public static readonly SaveEvents Save = new SaveEvents();
        public class SaveEvents
        {
            public UnityAction<UserDataCVD> OnProfileCreated;
            public UnityAction<string> OnProfileDeleted;

            public UnityAction<string> OnProfileUpdated;
        }

    }
}
