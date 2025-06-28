using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreEntry_UI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text rankText;     // Rang (#1, #2, etc.)
    public TMP_Text usernameText; // Benutzername  
    public TMP_Text scoreText;    // Score
    public TMP_Text dateText;     // Datum
    
    [Header("Visual Highlighting")]
    public Image backgroundImage; // Optional: Hintergrund für Top 3
    public Color goldColor = new Color(1f, 0.84f, 0f, 0.3f);     // Gold für Platz 1
    public Color silverColor = new Color(0.75f, 0.75f, 0.75f, 0.3f); // Silber für Platz 2
    public Color bronzeColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);    // Bronze für Platz 3
    
    void Start()
    {
        Debug.Log($"🏅 HighscoreEntry_UI Start() - GameObject: {gameObject.name}");
        
        // Debug: Komponenten prüfen
        Debug.Log($"   Highscore Entry Komponenten Check:");
        Debug.Log($"   Rank Text: {rankText != null} - {(rankText != null ? rankText.name : "NULL")}");
        Debug.Log($"   Username Text: {usernameText != null} - {(usernameText != null ? usernameText.name : "NULL")}");
        Debug.Log($"   Score Text: {scoreText != null} - {(scoreText != null ? scoreText.name : "NULL")}");
        Debug.Log($"   Date Text: {dateText != null} - {(dateText != null ? dateText.name : "NULL")}");
        Debug.Log($"   Background Image: {backgroundImage != null}");
    }
    
    public void SetupEntry(HighscoreEntry highscoreEntry, int rank)
    {
        Debug.Log($"🔧 SetupEntry() aufgerufen für Rang {rank}: {highscoreEntry.userName} - {highscoreEntry.finalScore}");
        
        // Rang setzen mit Emoji für Top 3
        if (rankText != null)
        {
            string rankDisplay = GetRankDisplay(rank);
            rankText.text = rankDisplay;
            Debug.Log($"🏅 Rank Text gesetzt: {rankDisplay}");
        }
        else
        {
            Debug.LogError("❌ Rank Text ist NULL!");
        }
        
        // Username setzen
        if (usernameText != null)
        {
            usernameText.text = highscoreEntry.userName;
            Debug.Log($"👤 Username Text gesetzt: {highscoreEntry.userName}");
        }
        else
        {
            Debug.LogError("❌ Username Text ist NULL!");
        }
        
        // Score setzen
        if (scoreText != null)
        {
            scoreText.text = highscoreEntry.finalScore.ToString();
            Debug.Log($"🎯 Score Text gesetzt: {highscoreEntry.finalScore}");
        }
        else
        {
            Debug.LogError("❌ Score Text ist NULL!");
        }
        
        // Datum setzen
        if (dateText != null)
        {
            string formattedDate = FormatDate(highscoreEntry.gameDate);
            dateText.text = formattedDate;
            Debug.Log($"📅 Date Text gesetzt: {formattedDate}");
        }
        else
        {
            Debug.LogError("❌ Date Text ist NULL!");
        }
        
        // Visuelle Hervorhebung für Top 3
        ApplyRankStyling(rank);
        
        // Aktueller User hervorheben
        HighlightCurrentUser(highscoreEntry.userName);
        
        Debug.Log($"✅ SetupEntry abgeschlossen für Rang {rank}: {highscoreEntry.userName}");
    }
    
    private string GetRankDisplay(int rank)
    {
        switch (rank)
        {
            case 1: return "🥇 1.";
            case 2: return "🥈 2.";
            case 3: return "🥉 3.";
            default: return $"{rank}.";
        }
    }
    
    private void ApplyRankStyling(int rank)
    {
        if (backgroundImage == null) return;
        
        // Hintergrundfarbe je nach Rang
        switch (rank)
        {
            case 1:
                backgroundImage.color = goldColor;
                Debug.Log("🥇 Gold-Styling angewendet");
                break;
            case 2:
                backgroundImage.color = silverColor;
                Debug.Log("🥈 Silber-Styling angewendet");
                break;
            case 3:
                backgroundImage.color = bronzeColor;
                Debug.Log("🥉 Bronze-Styling angewendet");
                break;
            default:
                backgroundImage.color = Color.clear; // Transparent für andere Ränge
                break;
        }
    }
    
    private void HighlightCurrentUser(string entryUserName)
    {
        // Aktuellen User hervorheben falls eingeloggt
        if (UserSessionManager.Instance != null && 
            !string.IsNullOrEmpty(UserSessionManager.Instance.LoggedInUsername))
        {
            string currentUser = UserSessionManager.Instance.LoggedInUsername;
            
            if (entryUserName.Equals(currentUser, System.StringComparison.OrdinalIgnoreCase))
            {
                // Aktueller User - hervorheben
                if (usernameText != null)
                {
                    usernameText.color = new Color(0.957f, 0.624f, 0.059f, 1f); // Orange #F49F0F
                    usernameText.fontStyle = FontStyles.Bold;
                    Debug.Log($"👑 Aktueller User hervorgehoben: {currentUser}");
                }
                
                // Optional: Rahmen oder Icon hinzufügen
                if (backgroundImage != null && backgroundImage.color == Color.clear)
                {
                    backgroundImage.color = new Color(0.957f, 0.624f, 0.059f, 0.1f); // Leicht orange
                }
            }
        }
    }
    
    private string FormatDate(string dateString)
    {
        Debug.Log($"📅 FormatDate Input: {dateString}");
        
        try
        {
            // Von "2025-06-28 20:56:52" zu "28.06.25"
            System.DateTime date = System.DateTime.Parse(dateString);
            string formatted = date.ToString("dd.MM.yy");
            Debug.Log($"✅ Datum erfolgreich formatiert: {dateString} → {formatted}");
            return formatted;
        }
        catch (System.Exception e)
        {
            // Fallback falls Parsing fehlschlägt
            string fallback = dateString.Substring(0, 8); // "2025-06-"
            Debug.LogWarning($"⚠️ Datum Parsing fehlgeschlagen: {dateString} → {fallback}");
            Debug.LogWarning($"🐛 Exception: {e.Message}");
            return fallback;
        }
    }
}