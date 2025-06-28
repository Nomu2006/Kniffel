using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameScore
{
    public string gameId;
    public string userName;
    public int finalScore;
    public string gameDate; // "2024-12-20 15:30:45"
    
    // Detaillierte Spielergebnisse
    public int upperSectionTotal;
    public int bonusPoints;
    public int lowerSectionTotal;
    
    // Kategorien-Details (optional für Statistiken)
    public int einer, zweier, dreier, vierer, fuenfer, sechser;
    public int dreierpasch, viererpasch, fullHouse, kleineStraße, großeStraße, kniffel, chance;
    
    public GameScore()
    {
        gameId = Guid.NewGuid().ToString();
        gameDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[Serializable]
public class UserScoreData
{
    public string userName;
    public List<GameScore> games = new List<GameScore>();
    public int totalGamesPlayed;
    public int bestScore;
    public float averageScore;
    
    public UserScoreData(string username)
    {
        userName = username;
        totalGamesPlayed = 0;
        bestScore = 0;
        averageScore = 0f;
    }
}