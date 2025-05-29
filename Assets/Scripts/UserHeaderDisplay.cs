using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserHeaderDisplay : MonoBehaviour
{
    public TMP_Text usernameLabel;
    public Image avatarImage;

    private void Start()
    {
        string username = UserSessionManager.Instance.LoggedInUsername;
        string avatarId = UserSessionManager.Instance.AvatarId;

        usernameLabel.text = username;

        Sprite avatarSprite = AvatarDatabase.Instance.GetAvatarSprite(avatarId);
        if (avatarSprite != null)
        {
            avatarImage.sprite = avatarSprite;
        }
        else
        {
            Debug.LogWarning("⚠ Avatar not found for ID: " + avatarId);
        }
    }
}