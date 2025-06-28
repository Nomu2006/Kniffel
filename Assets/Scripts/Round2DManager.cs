using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Round2DManager : MonoBehaviour
{
    [Header("UI References")]
    public Dice2DThrower diceThrower; // Referenz zum Dice Thrower
    public Transform holdDiceContainer; // Container für gehaltene Würfel
    public Text tryNumberLabel; // "3", "2", "1" Anzeige (veraltetes Text-System)
    public TMPro.TMP_Text tryNumberTMPLabel; // TMP Alternative für tryNumberLabel
    public Button rollButton; // Roll Button
    
    [Header("Round Settings")]
    public int maxTries = 3; // Maximum 3 Würfe pro Runde
    
    private int currentTries; // Aktuelle Anzahl verbleibender Würfe
    private int currentThrowCount = 0; // HINZUGEFÜGT: Anzahl bereits gemachter Würfe
    private List<GameObject> heldDice = new List<GameObject>(); // Gehaltene Würfel
    private List<int> heldDiceValues = new List<int>(); // Werte der gehaltenen Würfel
    private bool isRoundActive = false; // Ist eine Runde aktiv?
    
    void Start()
    {
        StartNewRound();
        
        // Button Event überschreiben - ABER ERST NACH dem Dice Thrower
        StartCoroutine(SetupButtonAfterFrame());
    }
    
    IEnumerator SetupButtonAfterFrame()
    {
        yield return null; // Warte einen Frame
        
        if (rollButton != null)
        {
            rollButton.onClick.RemoveAllListeners();
            rollButton.onClick.AddListener(HandleRollButton);
        }
    }
    
    void Update()
    {
        // Leertaste zum Würfeln (nur wenn Runde aktiv und nicht am würfeln)
        if (Input.GetKeyDown(KeyCode.Space) && isRoundActive && !diceThrower.IsRolling())
        {
            HandleRollButton();
        }
    }
    
    public void StartNewRound()
    {
        currentTries = maxTries;
        currentThrowCount = 0; // HINZUGEFÜGT: Reset der Wurf-Anzahl
        isRoundActive = true;
        
        // Alle gehaltenen Würfel zurück zum Spielfeld
        ReturnAllHeldDice();
        
        // UI Update
        UpdateTryCounter();
        UpdateRollButton();
        
        Debug.Log("Neue Runde gestartet! 3 Würfe verfügbar.");
    }
    
    // HINZUGEFÜGT: Methode zum manuellen Beenden der Runde (für Kategorie-Auswahl)
    public void EndCurrentRound()
    {
        isRoundActive = false;
        Debug.Log("Runde manuell beendet durch Kategorie-Auswahl");
    }
    
    // HINZUGEFÜGT: Getter für aktuelle Wurf-Anzahl
    public int GetCurrentThrowCount()
    {
        return currentThrowCount;
    }
    
    public void HandleRollButton()
    {
        if (!isRoundActive || diceThrower.IsRolling() || currentTries <= 0)
        {
            Debug.Log($"Roll blocked: RoundActive={isRoundActive}, IsRolling={diceThrower.IsRolling()}, Tries={currentTries}");
            return;
        }
        
        Debug.Log($"Rolling dice! Try {maxTries - currentTries + 1}/3");
        
        // Würfeln über den DiceThrower
        diceThrower.RollDice();
        
        // NACH dem Wurf den Versuch reduzieren
        StartCoroutine(HandleTryCountAfterRoll());
    }
    
    IEnumerator HandleTryCountAfterRoll()
    {
        // Warten bis Wurf fertig ist
        while (diceThrower.IsRolling())
        {
            yield return null;
        }
        
        // Versuch reduzieren und Wurf-Anzahl erhöhen
        currentTries--;
        currentThrowCount++; // HINZUGEFÜGT: Wurf-Anzahl erhöhen
        
        // UI Update
        UpdateTryCounter();
        UpdateRollButton();
        
        // Prüfen ob Runde beendet (nur wenn alle 3 Würfe verwendet)
        if (currentTries <= 0)
        {
            EndRound();
        }
        
        Debug.Log($"Wurf beendet. Verbleibende Versuche: {currentTries}, Gesamt Würfe: {currentThrowCount}");
    }
    
    IEnumerator EndRoundAfterAnimation()
    {
        // Warten bis Animation fertig ist
        while (diceThrower.IsRolling())
        {
            yield return null;
        }
        
        // Runde beenden
        EndRound();
    }
    
    void EndRound()
    {
        isRoundActive = false;
        
        Debug.Log("Runde beendet! Alle 3 Würfe verwendet.");
        Debug.Log("Finale Würfel: " + string.Join(", ", GetAllDiceValues()));
        
        // Hier könntest du weitere Logik hinzufügen:
        // - Punkte berechnen
        // - Nächste Runde starten
        // - etc.
    }
    
    public void MoveDiceToHold(GameObject dice)
    {
        if (!isRoundActive || dice == null) 
        {
            Debug.Log("Cannot move dice to hold - round not active or dice is null");
            return;
        }
        
        Debug.Log("Moving dice to hold container");
        
        // Würfel zum Hold Container bewegen
        dice.transform.SetParent(holdDiceContainer);
        
        // Zur gehaltenen Liste hinzufügen
        if (!heldDice.Contains(dice))
        {
            heldDice.Add(dice);
            Debug.Log($"Dice added to hold. Total held: {heldDice.Count}");
        }
        
        // Position im Hold Container anpassen
        RepositionHeldDice();
    }
    
    public void MoveDiceToField(GameObject dice)
    {
        if (!isRoundActive || dice == null) 
        {
            Debug.Log("Cannot move dice to field - round not active or dice is null");
            return;
        }
        
        Debug.Log("Moving dice back to field");
        
        // Würfel zurück zum Spielfeld
        dice.transform.SetParent(diceThrower.rollDiceFieldContainer);
        
        // Aus gehaltener Liste entfernen
        if (heldDice.Contains(dice))
        {
            heldDice.Remove(dice);
            Debug.Log($"Dice removed from hold. Total held: {heldDice.Count}");
        }
        
        // Positionen neu anordnen
        RepositionHeldDice();
    }
    
    void ReturnAllHeldDice()
    {
        // Alle gehaltenen Würfel zurück zum Spielfeld
        foreach (GameObject dice in heldDice)
        {
            if (dice != null)
            {
                dice.transform.SetParent(diceThrower.rollDiceFieldContainer);
            }
        }
        
        heldDice.Clear();
        heldDiceValues.Clear();
    }
    
    public List<GameObject> GetHeldDice()
    {
        return new List<GameObject>(heldDice);
    }
    
    void RepositionHeldDice()
    {
        // Null-Check für zerstörte Würfel
        heldDice.RemoveAll(dice => dice == null);
        
        if (heldDice.Count == 0) return;
        
        // Gehaltene Würfel im Hold Container anordnen
        RectTransform containerRect = holdDiceContainer.GetComponent<RectTransform>();
        if (containerRect == null) return;
        
        float containerWidth = containerRect.rect.width;
        float diceSize = 100f;
        float spacing = 110f;
        
        for (int i = 0; i < heldDice.Count; i++)
        {
            GameObject dice = heldDice[i];
            if (dice == null) continue;
            
            RectTransform diceRect = dice.GetComponent<RectTransform>();
            if (diceRect == null) continue;
            
            // Position berechnen (zentriert)
            float totalWidth = (heldDice.Count - 1) * spacing;
            float startX = -totalWidth / 2f;
            float posX = startX + i * spacing;
            
            diceRect.anchoredPosition = new Vector2(posX, 0);
        }
    }
    
    void UpdateTryCounter()
    {
        // Unterstützung für beide Text-Systeme
        if (tryNumberLabel != null)
        {
            tryNumberLabel.text = currentTries.ToString();
        }
        
        if (tryNumberTMPLabel != null)
        {
            tryNumberTMPLabel.text = currentTries.ToString();
        }
        
        Debug.Log($"Try Counter aktualisiert: {currentTries} verbleibende Würfe");
    }
    
    void UpdateRollButton()
    {
        if (rollButton != null)
        {
            // Button deaktivieren wenn keine Versuche mehr
            rollButton.interactable = currentTries > 0 && isRoundActive;
            
            // Button Text anpassen (unterstützt beide Text-Systeme)
            Text buttonText = rollButton.GetComponentInChildren<Text>();
            TMPro.TMP_Text buttonTMPText = rollButton.GetComponentInChildren<TMPro.TMP_Text>();
            
            string buttonTextContent;
            if (currentTries > 0)
            {
                buttonTextContent = $"ROLL ({currentTries})";
            }
            else
            {
                buttonTextContent = "NO ROLLS LEFT";
            }
            
            if (buttonText != null)
            {
                buttonText.text = buttonTextContent;
            }
            
            if (buttonTMPText != null)
            {
                buttonTMPText.text = buttonTextContent;
            }
        }
    }
    
    // Getter für alle Würfelwerte (gehaltene + auf dem Feld)
    public List<int> GetAllDiceValues()
    {
        List<int> allValues = new List<int>();
        
        // Gehaltene Würfel hinzufügen
        allValues.AddRange(heldDiceValues);
        
        // Würfel auf dem Feld hinzufügen
        allValues.AddRange(diceThrower.GetDiceResults());
        
        return allValues;
    }
    
    // Getter für Rundenstatus
    public bool IsRoundActive()
    {
        return isRoundActive;
    }
    
    public int GetRemainingTries()
    {
        return currentTries;
    }
}