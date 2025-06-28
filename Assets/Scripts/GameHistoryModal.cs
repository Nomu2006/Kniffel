using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameHistoryModal : MonoBehaviour
{
    [Header("Modal UI References")]
    public GameObject modalPanel; // Das Haupt-Modal Panel
    public GameObject modalBackground; // Dunkler Hintergrund
    
    [Header("Content")]
    public TMP_Text titleText; // "Game History"
    public Transform contentContainer; // Content von ScrollView
    public GameObject gameEntryPrefab; // Prefab für einzelne Spiel-Einträge
    
    [Header("Statistics")]
    public TMP_Text statsText; // Zeigt Gesamtstatistiken
    
    [Header("Scroll View")]
    public ScrollRect scrollRect; // ScrollView Component
    
    void Start()
    {
        Debug.Log("🔧 GameHistoryModal Start()");
        
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
            titleText.text = "Game History";
        }
        
        // Debug: Komponenten prüfen
        Debug.Log($"📋 Komponenten Check:");
        Debug.Log($"   Modal Panel: {modalPanel != null}");
        Debug.Log($"   Modal Background: {modalBackground != null}");
        Debug.Log($"   Title Text: {titleText != null}");
        Debug.Log($"   Content Container: {contentContainer != null}");
        Debug.Log($"   Game Entry Prefab: {gameEntryPrefab != null}");
        Debug.Log($"   Stats Text: {statsText != null}");
        Debug.Log($"   Scroll Rect: {scrollRect != null}");
    }
    
    // TOGGLE-Funktion für den Game History Button
    public void ToggleModal()
    {
        Debug.Log("🔄 ToggleModal() aufgerufen");
        
        // Prüfen ob Modal aktuell offen ist
        if (modalPanel != null && modalPanel.activeInHierarchy)
        {
            Debug.Log("📴 Modal ist offen - schließe es");
            CloseModal();
        }
        else
        {
            Debug.Log("📱 Modal ist geschlossen - öffne es");
            OpenModal();
        }
    }
    
    public void OpenModal()
    {
        Debug.Log("🚀 OpenModal() gestartet");
        
        // Prüfen ob User eingeloggt
        if (UserSessionManager.Instance == null)
        {
            Debug.LogError("❌ UserSessionManager.Instance ist NULL!");
            return;
        }
        
        if (string.IsNullOrEmpty(UserSessionManager.Instance.LoggedInUsername))
        {
            Debug.LogWarning("⚠️ Kein User eingeloggt!");
            return;
        }
        
        string currentUser = UserSessionManager.Instance.LoggedInUsername;
        Debug.Log($"👤 Aktueller User: {currentUser}");
        
        // Modal anzeigen
        ShowModal();
        
        // Spielhistorie laden und anzeigen
        LoadAndDisplayGameHistory(currentUser);
        
        Debug.Log($"✅ Game History Modal geöffnet für {currentUser}");
    }
    
    void ShowModal()
    {
        Debug.Log("📺 ShowModal() - Modal wird angezeigt");
        
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
        Debug.Log("📴 CloseModal() - Modal wird geschlossen");
        
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
        }
        
        Debug.Log("✅ Game History Modal geschlossen");
    }
    
    void LoadAndDisplayGameHistory(string userName)
    {
        Debug.Log($"📊 LoadAndDisplayGameHistory() für User: {userName}");
        
        // Vorherige Einträge löschen
        ClearPreviousEntries();
        
        // User-Daten laden
        if (UserScoreManager.Instance == null)
        {
            Debug.LogError("❌ UserScoreManager.Instance ist NULL!");
            ShowNoDataMessage("UserScoreManager nicht verfügbar");
            return;
        }
        
        Debug.Log("✅ UserScoreManager gefunden");
        
        UserScoreData userData = UserScoreManager.Instance.GetUserScoreData(userName);
        
        if (userData == null)
        {
            Debug.LogWarning($"⚠️ Keine UserScoreData für {userName} gefunden");
            ShowNoDataMessage("Noch keine Spiele gespielt");
            return;
        }
        
        Debug.Log($"📈 UserScoreData geladen: {userData.games.Count} Spiele gefunden");
        
        if (userData.games.Count == 0)
        {
            Debug.Log("📝 Keine Spiele in der Liste");
            ShowNoDataMessage("Noch keine Spiele gespielt");
            return;
        }
        
        // Statistiken anzeigen
        UpdateStatistics(userData);
        
        // Spiele anzeigen (neueste zuerst)
        var sortedGames = userData.games.OrderByDescending(game => game.gameDate).ToList();
        Debug.Log($"🔄 Spiele sortiert: {sortedGames.Count} Einträge");
        
        int entryCount = 0;
        foreach (GameScore game in sortedGames)
        {
            entryCount++;
            Debug.Log($"🎮 Erstelle Eintrag #{entryCount}: {game.gameDate} - Score: {game.finalScore}");
            CreateGameEntry(game);
        }
        
        // Scroll nach oben
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            Debug.Log("📜 Scroll Position auf Top gesetzt");
        }
        
        Debug.Log($"✅ Alle {entryCount} Einträge erstellt");
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
            if (child.gameObject != gameEntryPrefab)
            {
                Debug.Log($"🗑️ Lösche: {child.name}");
                Destroy(child.gameObject);
                destroyedCount++;
            }
        }
        
        Debug.Log($"✅ {destroyedCount} vorherige Einträge gelöscht");
    }
    
    void UpdateStatistics(UserScoreData userData)
    {
        if (statsText == null) 
        {
            Debug.LogError("❌ Stats Text ist NULL!");
            return;
        }
        
        string stats = $"📊 Statistiken:\n";
        stats += $"🎮 Spiele gespielt: {userData.totalGamesPlayed}\n";
        stats += $"🏆 Bester Score: {userData.bestScore}\n";
        stats += $"📈 Durchschnitt: {userData.averageScore:F1}";
        
        statsText.text = stats;
        Debug.Log($"📊 Statistiken aktualisiert: {userData.totalGamesPlayed} Spiele, Best: {userData.bestScore}");
    }
    
    void CreateGameEntry(GameScore game)
    {
        Debug.Log($"🏗️ CreateGameEntry() für: {game.gameDate}");
        
        if (gameEntryPrefab == null)
        {
            Debug.LogError("❌ Game Entry Prefab ist NULL!");
            return;
        }
        
        if (contentContainer == null)
        {
            Debug.LogError("❌ Content Container ist NULL!");
            return;
        }
        
        // Neuen Eintrag erstellen
        Debug.Log($"📦 Instantiate Prefab in Container: {contentContainer.name}");
        GameObject entryObj = Instantiate(gameEntryPrefab, contentContainer);
        entryObj.SetActive(true);
        Debug.Log($"✅ Entry GameObject erstellt: {entryObj.name}");
        
        // Game Entry Script holen und setup
        GameHistoryEntry entryScript = entryObj.GetComponent<GameHistoryEntry>();
        if (entryScript != null)
        {
            Debug.Log($"🎯 GameHistoryEntry Script gefunden - SetupEntry wird aufgerufen");
            entryScript.SetupEntry(game);
            Debug.Log($"✅ Entry Setup abgeschlossen");
        }
        else
        {
            Debug.LogError("❌ GameHistoryEntry Script nicht auf Prefab gefunden!");
            
            // Fallback: Direkt Text setzen
            TMP_Text[] texts = entryObj.GetComponentsInChildren<TMP_Text>();
            Debug.Log($"🔧 Fallback: {texts.Length} Text-Komponenten gefunden");
            
            if (texts.Length >= 3)
            {
                texts[0].text = FormatDate(game.gameDate);
                texts[1].text = game.userName;
                texts[2].text = game.finalScore.ToString();
                Debug.Log($"🔧 Fallback Text gesetzt: {texts[0].text} | {texts[1].text} | {texts[2].text}");
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
                Debug.Log("⌨️ ESC gedrückt - Modal wird geschlossen");
                CloseModal();
            }
        }
    }
}