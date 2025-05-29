using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UserLogin : MonoBehaviour
{
    [Header("Input Fields")] public TMP_InputField usernameLoginInput;
    public TMP_InputField passwordLoginInput;

    [Header("Error and Success Display")] public TMP_Text loginErrorText;
    public TMP_Text loginSuccessText;

    private string filePath;

    private void Start()
    {
        #if UNITY_EDITOR
        // Im Unity Editor – Datei liegt im Assets-Ordner
        filePath = Path.Combine(Application.dataPath, "Database/users.json");
        #else
        // Im Build – sichere, beschreibbare Pfade
        filePath = Path.Combine(Application.persistentDataPath, "users.json");
        #endif

        Debug.Log("User file path: " + filePath);
    }

    public void LoginUser()
    {
        string username = usernameLoginInput.text.Trim();
        string password = passwordLoginInput.text;

        if (ExistUser(username, password))
        {
            loginSuccessText.text = "Login Successful!";
            loginSuccessText.gameObject.SetActive(true);
            loginErrorText.text = "";
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            loginErrorText.text = "Username or password is incorrect.";
            loginSuccessText.gameObject.SetActive(false);
        }
    }

    public bool ExistUser(string username, string password)
    {
        // Datei einlesen
        string json = File.ReadAllText(filePath);
        UserList userList = JsonUtility.FromJson<UserList>(json);

        foreach (UserData user in userList.users)
        {
            if (user.userName.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                if (user.password == password)
                {
                    return true;
                }
                else
                {
                    Debug.Log(user.userName + " doesn't match");
                    return false; // Benutzername korrekt, Passwort falsch
                }
            }
        }

        Debug.Log("Username not found: " + username);
        return false; // Benutzername nicht gefunden
    }
}