using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameModalManager : MonoBehaviour
{
    [Header("Modal UI References")]
    public GameObject modalPanel; // Das Haupt-Modal Panel
    public GameObject modalBackground; // Dunkler Hintergrund (optional)
    
    [Header("Modal Content")]
    public TMP_Text currentScoreText; // Zeigt aktuelle Punkte
    public Button newGameButton; // New Game Button
    public Button exitButton; // Exit Button  
    public Button closeModalButton; // X oder Close Button
    
    [Header("Game References")]
    public PointCalculator2D pointCalculator; // Referenz zum PointCalculator
    public Button exitGameButton; // Der Exit-Button der das Modal öffnet
    
    [Header("Modal Animation (Optional)")]
    public bool useAnimation = false; // Auf false setzen wenn kein LeanTween
    public float animationDuration = 0.3f;
    
    void Start()
    {

        // Modal initial verstecken
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
        }
        
        // Button Events einrichten
        SetupButtonEvents();
    }
    
    void SetupButtonEvents()
    {
        // Exit Game Button (öffnet das Modal)
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OpenModal);
        }
        
        // Modal Buttons
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        
        if (closeModalButton != null)
        {
            closeModalButton.onClick.AddListener(CloseModal);
        }
    }
    
    public void OpenModal()
    {
        // Aktuelle Punkte anzeigen
        UpdateCurrentScore();
        
        // Modal anzeigen
        if (modalBackground != null)
        {
            modalBackground.SetActive(true);
        }
        
        if (modalPanel != null)
        {
            modalPanel.SetActive(true);
            
            // Optional: Animation
            if (useAnimation)
            {
                AnimateModalOpen();
            }
        }
        
        // Zeit pausieren (optional)
        Time.timeScale = 0f;
        
        Debug.Log("Modal geöffnet");
    }
    
    public void CloseModal()
    {
        // Zeit wieder normal
        Time.timeScale = 1f;
        
        // Modal verstecken
        if (useAnimation && modalPanel != null)
        {
            AnimateModalClose();
        }
        else
        {
            HideModal();
        }
        
        Debug.Log("Modal geschlossen");
    }
    
    void HideModal()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
        }
    }
    
    void UpdateCurrentScore()
    {
        if (currentScoreText != null && pointCalculator != null)
        {
            // Aktuelle Gesamtpunktzahl aus dem PointCalculator holen
            string currentScore = pointCalculator.gesamtpunktzahlText.text;
            
            if (string.IsNullOrEmpty(currentScore))
            {
                currentScore = "0";
            }
            
            currentScoreText.text = $"Score: {currentScore}";
        }
    }
    
    public void StartNewGame()
    {
        Debug.Log("Neues Spiel starten...");
        
        // Zeit wieder normal
        Time.timeScale = 1f;
        
        // Option 1: Szene neu laden (einfachste Lösung)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        // Option 2: Game Reset ohne Szene neu laden (wenn ResetGame() implementiert ist)
        /*
        if (pointCalculator != null)
        {
            pointCalculator.ResetGame();
        }
        CloseModal();
        */
    }
    
    public void ExitGame()
    {
        Debug.Log("Spiel beenden...");
        
        // Zeit wieder normal
        Time.timeScale = 1f;
        
        // Im Editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Im Build
            Application.Quit();
        #endif
    }
    
    // Einfache Animationen (ohne LeanTween)
    void AnimateModalOpen()
    {
        if (modalPanel != null)
        {
            // Einfach ohne Animation anzeigen
            modalPanel.transform.localScale = Vector3.one;
        }
    }
    
    void AnimateModalClose()
    {
        // Einfach ohne Animation verstecken
        HideModal();
    }
    
    // Update für ESC-Taste (optional)
    void Update()
    {
        // ESC zum Schließen des Modals
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (modalPanel != null && modalPanel.activeInHierarchy)
            {
                CloseModal();
            }
            else if (exitGameButton != null)
            {
                OpenModal();
            }
        }
    }
}