using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighscoreEntry
{
    public string gameId;
    public string userName;
    public int finalScore;
    public string gameDate; // "2024-12-20 15:30:45"
    public string avatarId; // Falls du Avatare hast
    
    // Detaillierte Spielergebnisse (optional für Statistiken)
    public int upperSectionTotal;
    public int bonusPoints;
    public int lowerSectionTotal;
    
    public HighscoreEntry()
    {
        gameId = Guid.NewGuid().ToString();
        gameDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public HighscoreEntry(GameScore gameScore)
    {
        gameId = gameScore.gameId;
        userName = gameScore.userName;
        finalScore = gameScore.finalScore;
        gameDate = gameScore.gameDate;
        upperSectionTotal = gameScore.upperSectionTotal;
        bonusPoints = gameScore.bonusPoints;
        lowerSectionTotal = gameScore.lowerSectionTotal;
        
        // Avatar aus UserSession holen
        if (UserSessionManager.Instance != null)
        {
            avatarId = UserSessionManager.Instance.AvatarId;
        }
    }
}

[Serializable]
public class HighscoreData
{
    public List<HighscoreEntry> topScores = new List<HighscoreEntry>();
    public int maxEntries = 10; // Top 10
    public string lastUpdated;
    
    public HighscoreData()
    {
        lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}