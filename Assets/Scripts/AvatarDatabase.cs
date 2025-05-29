using UnityEngine;
using System.Collections.Generic;

public class AvatarDatabase : MonoBehaviour
{
    public static AvatarDatabase Instance;

    [System.Serializable]
    public struct AvatarEntry
    {
        public string id;
        public Sprite sprite;
    }

    public AvatarEntry[] avatars;

    private Dictionary<string, Sprite> avatarDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        avatarDict = new Dictionary<string, Sprite>();
        foreach (var avatar in avatars)
        {
            avatarDict[avatar.id] = avatar.sprite;
        }
    }

    public Sprite GetAvatarSprite(string id)
    {
        if (avatarDict.ContainsKey(id))
            return avatarDict[id];

        Debug.LogWarning("⚠ Unknown avatar ID: " + id);
        return null;
    }
}