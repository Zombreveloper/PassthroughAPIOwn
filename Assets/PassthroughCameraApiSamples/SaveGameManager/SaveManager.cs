/* This class manages the UserDataCVD. This exists because PlayerPrefs doesn't provide support for lists... thanks for nothing.
 * Saves the data to a JSON File and retrieves it from there. Important to keep data between sessions.
 * Key for retrieving data will be the name set in UserDataCVD
 * 
 * Supposed to exist between Scene changes to prevent Data Loss. So Singleton and DontDestroyOnLoad is incentivised.
 * Not sure if that really is necessary but better be safe.
 */

using UnityEngine;
using System.IO;
using TMPro;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string filePath;

    public AllUserData Data = new AllUserData(); //Note: Maybe I don't need to make a new one everytime?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "userdataCVD.json");
            LoadData();
        }
        else Destroy(gameObject);
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
    }

    public void DeleteUser(string name)
    {
        int index = Data.Users.FindIndex(u => u.Name == name);
        if (index >= 0)
        {
            Data.Users.RemoveAt(index);
            SaveData();
        }
        else
        {
            Debug.LogWarning("Profile not found in JSON file. Something went seriously wrong...");
        }
    }

    public UserDataCVD GetName(string name)
    {
        return Data.Users.Find(u => u.Name == name);
    }

    public void SaveData()
    {
        string jsonForm = JsonUtility.ToJson(Data, true);
        File.WriteAllText(filePath, jsonForm);
    }

    public void LoadData()
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
}
