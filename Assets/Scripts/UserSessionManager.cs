using UnityEngine;

public class UserSessionManager : MonoBehaviour
{
    public static UserSessionManager Instance;

    public string LoggedInUsername;
    public string AvatarId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // bleibt über Szenen hinweg erhalten
    }

    public void Logout()
    {
        LoggedInUsername = null;
        AvatarId = null;
    }
}