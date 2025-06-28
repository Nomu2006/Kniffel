using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HighscoreModal : MonoBehaviour
{
    [Header("Modal UI References")]
    public GameObject modalPanel; // Das Haupt-Modal Panel
    public GameObject modalBackground; // Dunkler Hintergrund
    
    [Header("Content")]
    public TMP_Text titleText; // "Highscore Board"
    public Transform contentContainer; // Content von ScrollView
    public GameObject highscoreEntryPrefab; // Prefab für einzelne Highscore-Einträge
    
    [Header("Statistics")]
    public TMP_Text statsText; // Zeigt Gesamtstatistiken
    
    [Header("Scroll View")]
    public ScrollRect scrollRect; // ScrollView Component
    
    void Start()
    {
        Debug.Log("🏆 HighscoreModal Start()");
        
        // Modal initial verstecken
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
        }
        
        // Titel setzen (statisch)
        if (titleText != null)
        {
            titleText.text = "Highscore Board";
        }
        
        // Debug: Komponenten prüfen
        Debug.Log($"📋 Highscore Komponenten Check:");
        Debug.Log($"   Modal Panel: {modalPanel != null}");
        Debug.Log($"   Content Container: {contentContainer != null}");
        Debug.Log($"   Highscore Entry Prefab: {highscoreEntryPrefab != null}");
        Debug.Log($"   Stats Text: {statsText != null}");
    }
    
    // TOGGLE-Funktion für den Highscore Button
    public void ToggleModal()
    {
        Debug.Log("🔄 HighscoreModal ToggleModal() aufgerufen");
        
        // Prüfen ob Modal aktuell offen ist
        if (modalPanel != null && modalPanel.activeInHierarchy)
        {
            Debug.Log("📴 Highscore Modal ist offen - schließe es");
            CloseModal();
        }
        else
        {
            Debug.Log("📱 Highscore Modal ist geschlossen - öffne es");
            OpenModal();
        }
    }
    
    public void OpenModal()
    {
        Debug.Log("🚀 HighscoreModal OpenModal() gestartet");
        
        // Modal anzeigen
        ShowModal();
        
        // Highscore laden und anzeigen
        LoadAndDisplayHighscores();
        
        Debug.Log("✅ Highscore Modal geöffnet");
    }
    
    void ShowModal()
    {
        Debug.Log("📺 ShowModal() - Highscore Modal wird angezeigt");
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(true);
            Debug.Log("✅ Modal Background aktiviert");
        }
        
        if (modalPanel != null)
        {
            modalPanel.SetActive(true);
            Debug.Log("✅ Modal Panel aktiviert");
        }
    }
    
    public void CloseModal()
    {
        Debug.Log("📴 CloseModal() - Highscore Modal wird geschlossen");
        
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
        }
        
        Debug.Log("✅ Highscore Modal geschlossen");
    }
    
    void LoadAndDisplayHighscores()
    {
        Debug.Log("🏆 LoadAndDisplayHighscores() gestartet");
        
        // Vorherige Einträge löschen
        ClearPreviousEntries();
        
        // Highscore-Daten laden
        if (HighscoreManager.Instance == null)
        {
            Debug.LogError("❌ HighscoreManager.Instance ist NULL!");
            ShowNoDataMessage("HighscoreManager not available");
            return;
        }
        
        Debug.Log("✅ HighscoreManager gefunden");
        
        List<HighscoreEntry> topScores = HighscoreManager.Instance.GetTopScores(10);
        
        if (topScores == null || topScores.Count == 0)
        {
            Debug.Log("📝 Keine Highscores gefunden");
            ShowNoDataMessage("No highscores available yet");
            return;
        }
        
        Debug.Log($"📈 Highscores loaded: {topScores.Count} entries found");
        
        // Statistiken anzeigen
        UpdateStatistics(topScores);
        
        // Highscore-Einträge anzeigen
        int rank = 1;
        foreach (HighscoreEntry entry in topScores)
        {
            Debug.Log($"🏅 Erstelle Eintrag #{rank}: {entry.userName} - Score: {entry.finalScore}");
            CreateHighscoreEntry(entry, rank);
            rank++;
        }
        
        // Scroll nach oben
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            Debug.Log("📜 Scroll Position auf Top gesetzt");
        }
        
        Debug.Log($"✅ Alle {topScores.Count} Highscore-Einträge erstellt");
    }
    
    void ClearPreviousEntries()
    {
        if (contentContainer == null) 
        {
            Debug.LogError("❌ Content Container ist NULL!");
            return;
        }
        
        int childCount = contentContainer.childCount;
        Debug.Log($"🧹 ClearPreviousEntries: {childCount} Kinder gefunden");
        
        int destroyedCount = 0;
        // Alle Kinder löschen (außer dem Prefab selbst)
        foreach (Transform child in contentContainer)
        {
            if (child.gameObject != highscoreEntryPrefab)
            {
                Debug.Log($"🗑️ Lösche: {child.name}");
                Destroy(child.gameObject);
                destroyedCount++;
            }
        }
        
        Debug.Log($"✅ {destroyedCount} vorherige Einträge gelöscht");
    }
    
    void UpdateStatistics(List<HighscoreEntry> topScores)
    {
        if (statsText == null) 
        {
            Debug.LogError("❌ Stats Text ist NULL!");
            return;
        }
        
        string stats = $" Top 10 Highscore\n";
        
        if (topScores.Count > 0)
        {
            HighscoreEntry bestEntry = topScores.First();
            stats += $" Record: {bestEntry.finalScore} by {bestEntry.userName}\n";
            stats += $" Entries: {topScores.Count}/10\n";
            
            // Aktueller User-Rang (falls eingeloggt)
            if (UserSessionManager.Instance != null && !string.IsNullOrEmpty(UserSessionManager.Instance.LoggedInUsername))
            {
                string currentUser = UserSessionManager.Instance.LoggedInUsername;
                int playerRank = HighscoreManager.Instance.GetPlayerRank(currentUser);
                
                if (playerRank > 0)
                {
                    stats += $" Your rank: Place {playerRank}";
                }
                else
                {
                    stats += $" You are not in the Top 10 yet";
                }
            }
        }
        else
        {
            stats += "No highscores available yet";
        }
        
        statsText.text = stats;
        Debug.Log($"📊 Highscore-Statistiken aktualisiert: {topScores.Count} Einträge");
    }
    
    void CreateHighscoreEntry(HighscoreEntry entry, int rank)
    {
        Debug.Log($"🏗️ CreateHighscoreEntry() für Rang {rank}: {entry.userName}");
        
        if (highscoreEntryPrefab == null)
        {
            Debug.LogError("❌ Highscore Entry Prefab ist NULL!");
            return;
        }
        
        if (contentContainer == null)
        {
            Debug.LogError("❌ Content Container ist NULL!");
            return;
        }
        
        // Neuen Eintrag erstellen
        Debug.Log($"📦 Instantiate Prefab in Container: {contentContainer.name}");
        GameObject entryObj = Instantiate(highscoreEntryPrefab, contentContainer);
        entryObj.SetActive(true);
        Debug.Log($"✅ Entry GameObject erstellt: {entryObj.name}");
        
        // Highscore Entry Script holen und setup
        HighscoreEntry_UI entryScript = entryObj.GetComponent<HighscoreEntry_UI>();
        if (entryScript != null)
        {
            Debug.Log($"🎯 HighscoreEntry_UI Script gefunden - SetupEntry wird aufgerufen");
            entryScript.SetupEntry(entry, rank);
            Debug.Log($"✅ Entry Setup abgeschlossen");
        }
        else
        {
            Debug.LogError("❌ HighscoreEntry_UI Script nicht auf Prefab gefunden!");
            
            // Fallback: Direkt Text setzen
            TMP_Text[] texts = entryObj.GetComponentsInChildren<TMP_Text>();
            Debug.Log($"🔧 Fallback: {texts.Length} Text-Komponenten gefunden");
            
            if (texts.Length >= 4)
            {
                texts[0].text = rank.ToString(); // Rang
                texts[1].text = entry.userName; // Username
                texts[2].text = entry.finalScore.ToString(); // Score
                texts[3].text = FormatDate(entry.gameDate); // Datum
                Debug.Log($"🔧 Fallback Text gesetzt: {rank} | {entry.userName} | {entry.finalScore} | {FormatDate(entry.gameDate)}");
            }
        }
    }
    
    void ShowNoDataMessage(string message)
    {
        Debug.Log($"📝 ShowNoDataMessage: {message}");
        
        if (statsText != null)
        {
            statsText.text = message;
        }
    }
    
    string FormatDate(string dateString)
    {
        try
        {
            // Von "2025-06-28 20:56:52" zu "28.06.2025"
            System.DateTime date = System.DateTime.Parse(dateString);
            string formatted = date.ToString("dd.MM.yyyy");
            Debug.Log($"📅 Datum formatiert: {dateString} → {formatted}");
            return formatted;
        }
        catch
        {
            // Fallback falls Parsing fehlschlägt
            string fallback = dateString.Substring(0, 10);
            Debug.LogWarning($"⚠️ Datum Parsing fehlgeschlagen: {dateString} → {fallback}");
            return fallback;
        }
    }
    
    // Prüft ob das Modal aktuell offen ist
    public bool IsModalOpen()
    {
        return modalPanel != null && modalPanel.activeInHierarchy;
    }
    
    // Update für ESC-Taste
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsModalOpen())
            {
                Debug.Log("⌨️ ESC gedrückt - Highscore Modal wird geschlossen");
                CloseModal();
            }
        }
    }
}