using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UserRegister : MonoBehaviour
{
    [Header("Input Fields")] public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField passwordRepeatInput;

    [Header("Avatar Selection")]
    public Image avatarPreviewImage;
    public string[] avatarIds; // Nur noch IDs, z. B. "avatar_cat", "avatar_dog"


    private int currentAvatarIndex = 0;
    public string selectedAvatarId;

    [Header("Error Display")] public TMP_Text usernameErrorText;
    public TMP_Text passwordErrorText;
    public TMP_Text registrationSuccessText;


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

        UpdateAvatarPreview();
    }

    public void RegisterUser()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        string passwordRepeat = passwordRepeatInput.text;

        usernameErrorText.text = "";
        passwordErrorText.text = "";

        bool valid = true;

        if (!IsUsernameValid(username))
        {
            usernameErrorText.text = "Username invalid – please check the requirements.";
            valid = false;
        }
        else if (UsernameExists(username))
        {
            usernameErrorText.text = "Username already exists. Please choose another.";
            valid = false;
        }

        if (!IsPasswordValid(password))
        {
            passwordErrorText.text = "Password invalid – please review the requirements.";
            valid = false;
        }

        if (password != passwordRepeat)
        {
            passwordErrorText.text += "\nPasswords do not match.";
            valid = false;
        }

        if (!valid) return;

        // Lade bestehende Benutzer
        UserList userList = LoadUsers();

        // Prüfe auf bereits vorhandenen Benutzernamen
        foreach (var user in userList.users)
        {
            if (user.userName.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                usernameErrorText.text = "Username already exists.";
                return;
            }
        }

        // Erstelle neuen Benutzer
        UserData newUser = new UserData
        {
            userId = Guid.NewGuid().ToString(),
            userName = username,
            password = password,
            createdAt = DateTime.UtcNow.ToString("o"),
            avatarId = selectedAvatarId
        };

        userList.users.Add(newUser);
        SaveUsers(userList);

        // Felder leeren
        usernameInput.text = "";
        passwordInput.text = "";
        passwordRepeatInput.text = "";

        // Fehlertexte ausblenden
        usernameErrorText.text = "";
        passwordErrorText.text = "";

        // Erfolgsmeldung anzeigen
        registrationSuccessText.text = "Registration successful! Please log in.";
        registrationSuccessText.gameObject.SetActive(true);
        StartCoroutine(HideSuccessMessageAfterSeconds(4f));


        Debug.Log("✅ User registered: " + username);
    }

    private UserList LoadUsers()
    {
        if (!File.Exists(filePath))
            return new UserList();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<UserList>(json);
    }

    private void SaveUsers(UserList list)
    {
        string json = JsonUtility.ToJson(list, true);
        File.WriteAllText(filePath, json);
    }

    private bool IsUsernameValid(string username)
    {
        string pattern = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{6,}$";
        return Regex.IsMatch(username, pattern);
    }

    private bool IsPasswordValid(string password)
    {
        string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!§$?\/])[A-Za-z\d!§$?\/]{6,12}$";
        return Regex.IsMatch(password, pattern);
    }

    private bool UsernameExists(string usernameToCheck)
    {
        UserList existingUsers = LoadUsers();

        foreach (UserData user in existingUsers.users)
        {
            if (user.userName.Equals(usernameToCheck, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public void ShowPreviousAvatar()
    {
        currentAvatarIndex--;
        if (currentAvatarIndex < 0)
            currentAvatarIndex = avatarIds.Length - 1;

        UpdateAvatarPreview();
    }

    public void ShowNextAvatar()
    {
        currentAvatarIndex++;
        if (currentAvatarIndex >= avatarIds.Length)
            currentAvatarIndex = 0;

        UpdateAvatarPreview();
    }

    private void UpdateAvatarPreview()
    {
        selectedAvatarId = avatarIds[currentAvatarIndex];
        avatarPreviewImage.sprite = AvatarDatabase.Instance.GetAvatarSprite(selectedAvatarId);
    }
    
    private IEnumerator HideSuccessMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        registrationSuccessText.gameObject.SetActive(false);
    }
}