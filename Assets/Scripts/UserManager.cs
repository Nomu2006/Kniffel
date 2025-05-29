using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class UserData
{
    public string userId;
    public string userName;
    public string password;
    public string createdAt;
    public string avatarId;
}

[Serializable]
public class UserList
{
    public List<UserData> users = new List<UserData>();
}