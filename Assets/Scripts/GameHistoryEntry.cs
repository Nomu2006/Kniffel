using UnityEngine;
using TMPro;

public class GameHistoryEntry : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text dateText;     // Datum
    public TMP_Text usernameText; // Benutzername  
    public TMP_Text scoreText;    // Score
    
    void Start()
    {
        Debug.Log($"🎯 GameHistoryEntry Start() - GameObject: {gameObject.name}");
        
        // Debug: Komponenten prüfen
        Debug.Log($"📋 Entry Komponenten Check:");
        Debug.Log($"   Date Text: {dateText != null} - {(dateText != null ? dateText.name : "NULL")}");
        Debug.Log($"   Username Text: {usernameText != null} - {(usernameText != null ? usernameText.name : "NULL")}");
        Debug.Log($"   Score Text: {scoreText != null} - {(scoreText != null ? scoreText.name : "NULL")}");
    }
    
    public void SetupEntry(GameScore gameScore)
    {
        Debug.Log($"🔧 SetupEntry() aufgerufen für: {gameScore.userName} - {gameScore.finalScore}");
        
        if (dateText != null)
        {
            string formattedDate = FormatDate(gameScore.gameDate);
            dateText.text = formattedDate;
            Debug.Log($"📅 Date Text gesetzt: {formattedDate}");
        }
        else
        {
            Debug.LogError("❌ Date Text ist NULL!");
        }
        
        if (usernameText != null)
        {
            usernameText.text = gameScore.userName;
            Debug.Log($"👤 Username Text gesetzt: {gameScore.userName}");
        }
        else
        {
            Debug.LogError("❌ Username Text ist NULL!");
        }
        
        if (scoreText != null)
        {
            scoreText.text = gameScore.finalScore.ToString();
            Debug.Log($"🎯 Score Text gesetzt: {gameScore.finalScore}");
        }
        else
        {
            Debug.LogError("❌ Score Text ist NULL!");
        }
        
        Debug.Log($"✅ SetupEntry abgeschlossen für: {gameScore.userName}");
    }
    
    private string FormatDate(string dateString)
    {
        Debug.Log($"📅 FormatDate Input: {dateString}");
        
        try
        {
            // Von "2025-06-28 20:56:52" zu "28.06.2025"
            System.DateTime date = System.DateTime.Parse(dateString);
            string formatted = date.ToString("dd.MM.yyyy");
            Debug.Log($"✅ Datum erfolgreich formatiert: {dateString} → {formatted}");
            return formatted;
        }
        catch (System.Exception e)
        {
            // Fallback falls Parsing fehlschlägt
            string fallback = dateString.Substring(0, 10);
            Debug.LogWarning($"⚠️ Datum Parsing fehlgeschlagen: {dateString} → {fallback}");
            Debug.LogWarning($"🐛 Exception: {e.Message}");
            return fallback;
        }
    }
}