using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserScoreManager : MonoBehaviour
{
    public static UserScoreManager Instance;
    
    private string scoresDirectory;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDirectory();
    }
    
    private void InitializeDirectory()
    {
        #if UNITY_EDITOR
        // Im Unity Editor – Dateien liegen im Assets-Ordner
        scoresDirectory = Path.Combine(Application.dataPath, "Database/UserScores");
        #else
        // Im Build – sichere, beschreibbare Pfade
        scoresDirectory = Path.Combine(Application.persistentDataPath, "UserScores");
        #endif
        
        // Stelle sicher, dass das Verzeichnis existiert
        if (!Directory.Exists(scoresDirectory))
        {
            Directory.CreateDirectory(scoresDirectory);
            Debug.Log("UserScores Verzeichnis erstellt: " + scoresDirectory);
        }
        
        Debug.Log("UserScores Verzeichnis: " + scoresDirectory);
    }
    
    public void SaveGameScore(PointCalculator2D pointCalculator)
    {
        // Prüfen ob ein User eingeloggt ist
        if (UserSessionManager.Instance == null || string.IsNullOrEmpty(UserSessionManager.Instance.LoggedInUsername))
        {
            Debug.LogWarning("Kein User eingeloggt - Score wird nicht gespeichert!");
            return;
        }
        
        string userName = UserSessionManager.Instance.LoggedInUsername;
        
        // Neuen GameScore erstellen
        GameScore newScore = CreateGameScoreFromCalculator(pointCalculator, userName);
        
        // User-spezifische Datei laden oder erstellen
        UserScoreData userScoreData = LoadUserScoreData(userName);
        
        // Neuen Score hinzufügen
        userScoreData.games.Add(newScore);
        
        // Statistiken aktualisieren
        UpdateUserStatistics(userScoreData);
        
        // Speichern
        SaveUserScoreData(userScoreData);
        
        Debug.Log($"✅ Score gespeichert für {userName}: {newScore.finalScore} Punkte in Datei {userName}_scores.json");
    }
    
    private GameScore CreateGameScoreFromCalculator(PointCalculator2D calc, string userName)
    {
        GameScore score = new GameScore();
        score.userName = userName;
        
        // Finale Punkte
        int.TryParse(calc.gesamtpunktzahlText.text, out score.finalScore);
        
        // Upper Section
        int.TryParse(calc.gesamtUpperText.text, out score.upperSectionTotal);
        int.TryParse(calc.bonusText.text, out score.bonusPoints);
        
        // Lower Section
        score.lowerSectionTotal = score.finalScore - score.upperSectionTotal - score.bonusPoints;
        
        // Einzelne Kategorien (für detaillierte Statistiken)
        int.TryParse(calc.einerInput.text, out score.einer);
        int.TryParse(calc.zweierInput.text, out score.zweier);
        int.TryParse(calc.dreierInput.text, out score.dreier);
        int.TryParse(calc.viererInput.text, out score.vierer);
        int.TryParse(calc.fuenferInput.text, out score.fuenfer);
        int.TryParse(calc.sechserInput.text, out score.sechser);
        
        int.TryParse(calc.dreierpaschenInput.text, out score.dreierpasch);
        int.TryParse(calc.viererpaschenInput.text, out score.viererpasch);
        int.TryParse(calc.fullHouseInput.text, out score.fullHouse);
        int.TryParse(calc.kleineStraßeInput.text, out score.kleineStraße);
        int.TryParse(calc.großeStraßeInput.text, out score.großeStraße);
        int.TryParse(calc.kniffelInput.text, out score.kniffel);
        int.TryParse(calc.chanceInput.text, out score.chance);
        
        return score;
    }
    
    private void UpdateUserStatistics(UserScoreData userData)
    {
        userData.totalGamesPlayed = userData.games.Count;
        
        if (userData.games.Count > 0)
        {
            userData.bestScore = userData.games.Max(game => game.finalScore);
            userData.averageScore = (float)userData.games.Average(game => game.finalScore);
        }
    }
    
    private UserScoreData LoadUserScoreData(string userName)
    {
        string filePath = GetUserScoreFilePath(userName);
        
        if (!File.Exists(filePath))
        {
            return new UserScoreData(userName);
        }
        
        try
        {
            string json = File.ReadAllText(filePath);
            UserScoreData data = JsonUtility.FromJson<UserScoreData>(json);
            
            // Sicherstellen, dass die Liste nicht null ist
            if (data.games == null)
            {
                data.games = new List<GameScore>();
            }
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Laden der User-Scores für {userName}: " + e.Message);
            return new UserScoreData(userName);
        }
    }
    
    private void SaveUserScoreData(UserScoreData userData)
    {
        string filePath = GetUserScoreFilePath(userData.userName);
        
        try
        {
            string json = JsonUtility.ToJson(userData, true);
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Speichern der User-Scores für {userData.userName}: " + e.Message);
        }
    }
    
    private string GetUserScoreFilePath(string userName)
    {
        // Dateiname: username_scores.json
        string fileName = $"{userName}_scores.json";
        return Path.Combine(scoresDirectory, fileName);
    }
    
    // Public Methoden für Statistiken
    public UserScoreData GetUserScoreData(string userName)
    {
        return LoadUserScoreData(userName);
    }
    
    public GameScore GetBestScoreForUser(string userName)
    {
        UserScoreData userData = LoadUserScoreData(userName);
        return userData.games.OrderByDescending(game => game.finalScore).FirstOrDefault();
    }
    
    public List<GameScore> GetRecentGamesForUser(string userName, int count = 10)
    {
        UserScoreData userData = LoadUserScoreData(userName);
        return userData.games.OrderByDescending(game => game.gameDate).Take(count).ToList();
    }
    
    public int GetTotalGamesPlayedByUser(string userName)
    {
        UserScoreData userData = LoadUserScoreData(userName);
        return userData.totalGamesPlayed;
    }
    
    public float GetAverageScoreForUser(string userName)
    {
        UserScoreData userData = LoadUserScoreData(userName);
        return userData.averageScore;
    }
    
    // Hilfsmethoden für Ranglisten
    public List<UserScoreData> GetAllUserScoreData()
    {
        List<UserScoreData> allUserData = new List<UserScoreData>();
        
        if (!Directory.Exists(scoresDirectory))
            return allUserData;
        
        string[] scoreFiles = Directory.GetFiles(scoresDirectory, "*_scores.json");
        
        foreach (string filePath in scoreFiles)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                UserScoreData userData = JsonUtility.FromJson<UserScoreData>(json);
                allUserData.Add(userData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Fehler beim Laden von {filePath}: " + e.Message);
            }
        }
        
        return allUserData;
    }
    
    public List<UserScoreData> GetTopPlayersByBestScore(int count = 10)
    {
        List<UserScoreData> allUsers = GetAllUserScoreData();
        return allUsers.OrderByDescending(user => user.bestScore).Take(count).ToList();
    }
}