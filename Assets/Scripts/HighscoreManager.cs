using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance;
    
    private string highscoreFilePath;
    private const string HIGHSCORE_FILENAME = "highscores.json";
    
    [Header("Highscore Settings")]
    public int maxHighscoreEntries = 10; // Top 10
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeFilePath();
    }
    
    private void InitializeFilePath()
    {
        #if UNITY_EDITOR
        // Im Unity Editor – Datei liegt im Assets-Ordner
        string directory = Path.Combine(Application.dataPath, "Database");
        #else
        // Im Build – sichere, beschreibbare Pfade
        string directory = Application.persistentDataPath;
        #endif
        
        // Stelle sicher, dass das Verzeichnis existiert
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        highscoreFilePath = Path.Combine(directory, HIGHSCORE_FILENAME);
        
        Debug.Log("Highscore file path: " + highscoreFilePath);
    }
    
    public void AddScore(GameScore gameScore)
    {
        // Prüfen ob gültiger Score
        if (gameScore == null || gameScore.finalScore <= 0)
        {
            Debug.LogWarning("Ungültiger GameScore für Highscore!");
            return;
        }
        
        // Bestehende Highscores laden
        HighscoreData highscoreData = LoadHighscoreData();
        
        // Neuen Entry erstellen
        HighscoreEntry newEntry = new HighscoreEntry(gameScore);
        
        // Zur Liste hinzufügen
        highscoreData.topScores.Add(newEntry);
        
        // Nach Score sortieren (höchste zuerst)
        highscoreData.topScores = highscoreData.topScores
            .OrderByDescending(entry => entry.finalScore)
            .Take(maxHighscoreEntries) // Nur Top 10 behalten
            .ToList();
        
        // Aktualisierungsdatum setzen
        highscoreData.lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // Speichern
        SaveHighscoreData(highscoreData);
        
        // Prüfen ob neuer Highscore
        bool isNewHighscore = IsNewHighscore(newEntry, highscoreData);
        
        Debug.Log($"✅ Score zu Highscore hinzugefügt: {gameScore.userName} - {gameScore.finalScore} Punkte");
        
        if (isNewHighscore)
        {
            Debug.Log($"🎉 NEUER HIGHSCORE! {gameScore.userName} erreicht Platz {GetScoreRank(newEntry, highscoreData)}");
        }
    }
    
    private HighscoreData LoadHighscoreData()
    {
        if (!File.Exists(highscoreFilePath))
        {
            Debug.Log("Highscore-Datei existiert nicht - erstelle neue");
            return new HighscoreData();
        }
        
        try
        {
            string json = File.ReadAllText(highscoreFilePath);
            HighscoreData data = JsonUtility.FromJson<HighscoreData>(json);
            
            // Sicherstellen, dass die Liste nicht null ist
            if (data.topScores == null)
            {
                data.topScores = new List<HighscoreEntry>();
            }
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Laden der Highscores: " + e.Message);
            return new HighscoreData();
        }
    }
    
    private void SaveHighscoreData(HighscoreData highscoreData)
    {
        try
        {
            string json = JsonUtility.ToJson(highscoreData, true);
            File.WriteAllText(highscoreFilePath, json);
            
            Debug.Log($"💾 Highscores gespeichert: {highscoreData.topScores.Count} Einträge");
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Speichern der Highscores: " + e.Message);
        }
    }
    
    private bool IsNewHighscore(HighscoreEntry newEntry, HighscoreData highscoreData)
    {
        // Wenn weniger als 10 Einträge, ist es automatisch ein Highscore
        if (highscoreData.topScores.Count < maxHighscoreEntries)
        {
            return true;
        }
        
        // Prüfen ob der Score in den Top 10 ist
        int lowestTopScore = highscoreData.topScores.Last().finalScore;
        return newEntry.finalScore > lowestTopScore;
    }
    
    private int GetScoreRank(HighscoreEntry entry, HighscoreData highscoreData)
    {
        var sortedScores = highscoreData.topScores.OrderByDescending(s => s.finalScore).ToList();
        
        for (int i = 0; i < sortedScores.Count; i++)
        {
            if (sortedScores[i].gameId == entry.gameId)
            {
                return i + 1; // Rank is 1-based
            }
        }
        
        return -1; // Nicht gefunden
    }
    
    // Public Methoden für UI/Display
    public List<HighscoreEntry> GetTopScores(int count = 10)
    {
        HighscoreData data = LoadHighscoreData();
        return data.topScores.Take(count).ToList();
    }
    
    public HighscoreEntry GetBestScore()
    {
        HighscoreData data = LoadHighscoreData();
        return data.topScores.FirstOrDefault();
    }
    
    public int GetPlayerRank(string userName)
    {
        HighscoreData data = LoadHighscoreData();
        
        for (int i = 0; i < data.topScores.Count; i++)
        {
            if (data.topScores[i].userName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                return i + 1;
            }
        }
        
        return -1; // Nicht in Top 10
    }
    
    public bool IsScoreInTopTen(int score)
    {
        HighscoreData data = LoadHighscoreData();
        
        if (data.topScores.Count < maxHighscoreEntries)
        {
            return true;
        }
        
        int lowestTopScore = data.topScores.Last().finalScore;
        return score > lowestTopScore;
    }
    
    public int GetTotalScoresCount()
    {
        HighscoreData data = LoadHighscoreData();
        return data.topScores.Count;
    }
    
    // Debug-Methoden
    public void ClearHighscores()
    {
        HighscoreData emptyData = new HighscoreData();
        SaveHighscoreData(emptyData);
        Debug.Log("🗑️ Alle Highscores gelöscht");
    }
    
    public void PrintTopScores()
    {
        List<HighscoreEntry> topScores = GetTopScores();
        
        Debug.Log("🏆 TOP 10 HIGHSCORES:");
        for (int i = 0; i < topScores.Count; i++)
        {
            var entry = topScores[i];
            Debug.Log($"{i + 1}. {entry.userName}: {entry.finalScore} ({entry.gameDate})");
        }
    }
}