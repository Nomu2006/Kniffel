using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointCalculator2D : MonoBehaviour
{
    [Header("Game Manager References")]
    public Round2DManager roundManager;
    public Dice2DThrower diceThrower;

    [Header("Upper Section")]
    public TMP_InputField einerInput;
    public TMP_InputField zweierInput;
    public TMP_InputField dreierInput;
    public TMP_InputField viererInput;
    public TMP_InputField fuenferInput;
    public TMP_InputField sechserInput;
    public TMP_InputField gesamtUpperText;
    public TMP_InputField bonusText;

    [Header("Lower Section")]
    public TMP_InputField dreierpaschenInput;
    public TMP_InputField viererpaschenInput;
    public TMP_InputField fullHouseInput;
    public TMP_InputField kleineStraßeInput;
    public TMP_InputField großeStraßeInput;
    public TMP_InputField kniffelInput;
    public TMP_InputField chanceInput;

    [Header("Total")]
    public TMP_InputField gesamtpunktzahlText;

    [Header("Row Buttons")]
    public Button einerRowButton;
    public Button zweierRowButton;
    public Button dreierRowButton;
    public Button viererRowButton;
    public Button fuenferRowButton;
    public Button sechserRowButton;
    public Button dreierpaschenRowButton;
    public Button viererpaschenRowButton;
    public Button fullHouseRowButton;
    public Button kleineStraßeRowButton;
    public Button großeStraßeRowButton;
    public Button kniffelRowButton;
    public Button chanceRowButton;

    [Header("Game Over Modal")]
    public GameModalManager gameModalManager; // Referenz zum Modal Manager

    [Header("Visual Highlighting Settings")]
    [SerializeField] private bool enableVisualHighlighting = true;
    [SerializeField] private Color highlightColor = new Color(0.957f, 0.624f, 0.059f, 1f); // #F49F0F
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color disabledButtonColor = Color.gray;
    [SerializeField] private Color normalTextColor = Color.black;

    // Tracking welche Kategorien bereits verwendet wurden
    private bool[] usedCategories = new bool[13]; // 6 upper + 7 lower
    private int filledCategories = 0;

    // Aktuelle Würfelwerte
    private List<int> currentDiceValues = new List<int>();

    // Arrays für einfachen Zugriff auf UI Elemente
    private Button[] rowButtons;
    private TMP_InputField[] inputFields;

    // Original Farben speichern
    private ColorBlock[] originalButtonColors;
    private Color[] originalInputTextColors;

    void Start()
    {
        // Arrays initialisieren
        InitializeUIArrays();

        // Original Farben speichern
        SaveOriginalColors();

        // Button Events setup
        SetupRowButtons();

        // Alle InputFields zu Beginn read-only
        SetAllFieldsReadOnly();

        // Neue Runde starten
        StartNewRound();
    }

    void InitializeUIArrays()
    {
        rowButtons = new Button[] {
            einerRowButton, zweierRowButton, dreierRowButton, viererRowButton,
            fuenferRowButton, sechserRowButton, dreierpaschenRowButton,
            viererpaschenRowButton, fullHouseRowButton, kleineStraßeRowButton,
            großeStraßeRowButton, kniffelRowButton, chanceRowButton
        };

        inputFields = new TMP_InputField[] {
            einerInput, zweierInput, dreierInput, viererInput, fuenferInput, sechserInput,
            dreierpaschenInput, viererpaschenInput, fullHouseInput, kleineStraßeInput,
            großeStraßeInput, kniffelInput, chanceInput
        };
    }

    void SaveOriginalColors()
    {
        // Original Button Farben speichern
        originalButtonColors = new ColorBlock[rowButtons.Length];
        for (int i = 0; i < rowButtons.Length; i++)
        {
            if (rowButtons[i] != null)
            {
                originalButtonColors[i] = rowButtons[i].colors;
            }
        }

        // Original InputField Text Farben speichern
        originalInputTextColors = new Color[inputFields.Length];
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] != null && inputFields[i].textComponent != null)
            {
                originalInputTextColors[i] = inputFields[i].textComponent.color;
            }
        }
    }

    void Update()
    {
        // Würfelwerte aktualisieren wenn nicht am würfeln
        if (roundManager != null && !diceThrower.IsRolling())
        {
            UpdateCurrentDiceValues();
            UpdateCategoryButtons(); // Buttons nach jedem Wurf aktualisieren
        }
    }

    void SetupRowButtons()
    {
        // Row Buttons mit onClick Events verbinden
        if (einerRowButton != null) einerRowButton.onClick.AddListener(SelectEiner);
        if (zweierRowButton != null) zweierRowButton.onClick.AddListener(SelectZweier);
        if (dreierRowButton != null) dreierRowButton.onClick.AddListener(SelectDreier);
        if (viererRowButton != null) viererRowButton.onClick.AddListener(SelectVierer);
        if (fuenferRowButton != null) fuenferRowButton.onClick.AddListener(SelectFuenfer);
        if (sechserRowButton != null) sechserRowButton.onClick.AddListener(SelectSechser);
        if (dreierpaschenRowButton != null) dreierpaschenRowButton.onClick.AddListener(SelectDreierpasch);
        if (viererpaschenRowButton != null) viererpaschenRowButton.onClick.AddListener(SelectViererpasch);
        if (fullHouseRowButton != null) fullHouseRowButton.onClick.AddListener(SelectFullHouse);
        if (kleineStraßeRowButton != null) kleineStraßeRowButton.onClick.AddListener(SelectKleineStraße);
        if (großeStraßeRowButton != null) großeStraßeRowButton.onClick.AddListener(SelectGroßeStraße);
        if (kniffelRowButton != null) kniffelRowButton.onClick.AddListener(SelectKniffel);
        if (chanceRowButton != null) chanceRowButton.onClick.AddListener(SelectChance);
    }

    public void StartNewRound()
    {
        if (roundManager != null)
        {
            roundManager.StartNewRound();
        }

        // Buttons je nach Verfügbarkeit aktivieren/deaktivieren
        UpdateCategoryButtons();

        Debug.Log("Neue Kniffel-Runde gestartet!");
    }

    void UpdateCurrentDiceValues()
    {
        if (roundManager != null)
        {
            currentDiceValues = roundManager.GetAllDiceValues();

            // Debug: Zeige mögliche Punkte nach jedem Wurf
            if (currentDiceValues.Count == 5)
            {
                ShowPossibleScores();
            }
        }
    }

    public void SelectCategory(int categoryIndex)
    {
        // Prüfen ob Kategorie bereits verwendet
        if (usedCategories[categoryIndex])
        {
            Debug.Log($"Kategorie {categoryIndex} bereits verwendet!");
            return;
        }

        // Prüfen ob überhaupt Würfelwerte vorhanden sind (mindestens ein Wurf gemacht)
        if (currentDiceValues.Count != 5)
        {
            Debug.Log("Noch kein Wurf gemacht - würfle zuerst!");
            return;
        }

        // Prüfen ob der Spieler überhaupt schon gewürfelt hat in dieser Runde
        if (roundManager != null && roundManager.GetCurrentThrowCount() == 0)
        {
            Debug.Log("Noch kein Wurf in dieser Runde gemacht!");
            return;
        }

        // Punkte berechnen und eintragen
        int points = CalculatePointsForCategory(categoryIndex);
        SetCategoryPoints(categoryIndex, points);

        // Kategorie als verwendet markieren
        usedCategories[categoryIndex] = true;
        filledCategories++;

        // Runde explizit beenden wenn Kategorie gewählt wird
        if (roundManager != null)
        {
            roundManager.EndCurrentRound();
        }

        // UI aktualisieren
        UpdateCategoryButtons();
        CalculateTotal();

        Debug.Log($"Kategorie {GetCategoryName(categoryIndex)} gewählt nach {roundManager?.GetCurrentThrowCount() ?? 0} Würfen: {points} Punkte");

        // Prüfen ob Spiel beendet
        if (filledCategories >= 13)
        {
            EndGame();
        }
        else
        {
            // Nächste Runde starten
            StartNewRound();
        }
    }

    void UpdateCategoryButtons()
    {
        // Prüfen ob bereits ein Wurf in der aktuellen Runde gemacht wurde
        bool hasThrown = roundManager != null && roundManager.GetCurrentThrowCount() > 0 && currentDiceValues.Count == 5;

        for (int i = 0; i < rowButtons.Length; i++)
        {
            if (rowButtons[i] != null)
            {
                // Button ist nur verfügbar wenn:
                // 1. Die Kategorie noch nicht verwendet wurde UND
                // 2. Mindestens ein Wurf gemacht wurde
                bool isAvailable = !usedCategories[i] && hasThrown;
                bool isUsed = usedCategories[i];

                // Button Interactability setzen
                rowButtons[i].interactable = isAvailable;

                // Visuelles Highlighting nur wenn aktiviert
                if (enableVisualHighlighting)
                {
                    // Prüfen ob diese Kategorie Punkte bringen würde
                    bool shouldHighlight = false;
                    if (hasThrown && !usedCategories[i])
                    {
                        int points = CalculatePointsForCategory(i);
                        shouldHighlight = points > 0;
                    }

                    // Button visuell anpassen
                    UpdateButtonVisuals(i, isAvailable, shouldHighlight, isUsed);

                    // InputField visuell anpassen
                    if (inputFields[i] != null)
                    {
                        UpdateInputFieldVisuals(i, shouldHighlight, isUsed);
                    }
                }
                else
                {
                    // Standard Verhalten ohne Highlighting
                    RestoreOriginalButtonVisuals(i, isAvailable, isUsed);
                    if (inputFields[i] != null)
                    {
                        RestoreOriginalInputFieldVisuals(i);
                    }
                }
            }
        }
    }

    void UpdateButtonVisuals(int index, bool isAvailable, bool shouldHighlight, bool isUsed)
    {
        Button button = rowButtons[index];
        ColorBlock colors = originalButtonColors[index];

        if (isUsed)
        {
            // Bereits verwendet - grau
            colors.normalColor = disabledButtonColor;
            colors.disabledColor = disabledButtonColor;
        }
        else if (!isAvailable)
        {
            // Noch nicht gewürfelt - dunkelgrau
            colors.normalColor = disabledButtonColor * 0.7f;
            colors.disabledColor = disabledButtonColor * 0.7f;
        }
        else if (shouldHighlight)
        {
            // Verfügbar und bringt Punkte - highlight color
            colors.normalColor = highlightColor;
            colors.highlightedColor = highlightColor * 1.2f;
            colors.selectedColor = highlightColor * 0.8f;
        }
        else
        {
            // Verfügbar aber bringt keine Punkte - original colors
            colors = originalButtonColors[index];
        }

        button.colors = colors;

        // Button Text färben
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        Text legacyButtonText = button.GetComponentInChildren<Text>();

        Color textColor = shouldHighlight && !isUsed && isAvailable ? highlightColor : normalTextColor;

        if (buttonText != null)
        {
            buttonText.color = textColor;
        }
        if (legacyButtonText != null)
        {
            legacyButtonText.color = textColor;
        }
    }

    void UpdateInputFieldVisuals(int index, bool shouldHighlight, bool isUsed)
    {
        TMP_InputField inputField = inputFields[index];

        // InputField Text färben
        if (shouldHighlight && !isUsed)
        {
            inputField.textComponent.color = highlightColor;
        }
        else
        {
            inputField.textComponent.color = originalInputTextColors[index];
        }
    }

    void RestoreOriginalButtonVisuals(int index, bool isAvailable, bool isUsed)
    {
        Button button = rowButtons[index];
        ColorBlock colors = originalButtonColors[index];

        if (isUsed)
        {
            colors.normalColor = disabledButtonColor;
            colors.disabledColor = disabledButtonColor;
        }
        else if (!isAvailable)
        {
            colors.normalColor = disabledButtonColor * 0.7f;
            colors.disabledColor = disabledButtonColor * 0.7f;
        }

        button.colors = colors;

        // Text Farbe zurücksetzen
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        Text legacyButtonText = button.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
        if (legacyButtonText != null)
        {
            legacyButtonText.color = normalTextColor;
        }
    }

    void RestoreOriginalInputFieldVisuals(int index)
    {
        TMP_InputField inputField = inputFields[index];
        inputField.textComponent.color = originalInputTextColors[index];
    }

    // Alle anderen Methoden bleiben unverändert...
    int CalculatePointsForCategory(int categoryIndex)
    {
        switch (categoryIndex)
        {
            case 0: return CalculateEiner();
            case 1: return CalculateZweier();
            case 2: return CalculateDreier();
            case 3: return CalculateVierer();
            case 4: return CalculateFuenfer();
            case 5: return CalculateSechser();
            case 6: return CalculateDreierpasch();
            case 7: return CalculateViererpasch();
            case 8: return CalculateFullHouse();
            case 9: return CalculateKleineStraße();
            case 10: return CalculateGroßeStraße();
            case 11: return CalculateKniffel();
            case 12: return CalculateChance();
            default: return 0;
        }
    }

    void SetCategoryPoints(int categoryIndex, int points)
    {
        switch (categoryIndex)
        {
            case 0: einerInput.text = points.ToString(); break;
            case 1: zweierInput.text = points.ToString(); break;
            case 2: dreierInput.text = points.ToString(); break;
            case 3: viererInput.text = points.ToString(); break;
            case 4: fuenferInput.text = points.ToString(); break;
            case 5: sechserInput.text = points.ToString(); break;
            case 6: dreierpaschenInput.text = points.ToString(); break;
            case 7: viererpaschenInput.text = points.ToString(); break;
            case 8: fullHouseInput.text = points.ToString(); break;
            case 9: kleineStraßeInput.text = points.ToString(); break;
            case 10: großeStraßeInput.text = points.ToString(); break;
            case 11: kniffelInput.text = points.ToString(); break;
            case 12: chanceInput.text = points.ToString(); break;
        }
    }

    string GetCategoryName(int categoryIndex)
    {
        string[] names = {"Einer", "Zweier", "Dreier", "Vierer", "Fünfer", "Sechser",
                         "Dreierpasch", "Viererpasch", "Full House", "Kleine Straße",
                         "Große Straße", "Kniffel", "Chance"};
        return categoryIndex < names.Length ? names[categoryIndex] : "Unbekannt";
    }

    void SetAllFieldsReadOnly()
    {
        // Alle InputFields auf read-only setzen
        einerInput.readOnly = true;
        zweierInput.readOnly = true;
        dreierInput.readOnly = true;
        viererInput.readOnly = true;
        fuenferInput.readOnly = true;
        sechserInput.readOnly = true;
        gesamtUpperText.readOnly = true;
        bonusText.readOnly = true;
        dreierpaschenInput.readOnly = true;
        viererpaschenInput.readOnly = true;
        fullHouseInput.readOnly = true;
        kleineStraßeInput.readOnly = true;
        großeStraßeInput.readOnly = true;
        kniffelInput.readOnly = true;
        chanceInput.readOnly = true;
        gesamtpunktzahlText.readOnly = true;
    }

    // Berechnungsmethoden (unverändert)
    public int CalculateEiner() => currentDiceValues.Where(d => d == 1).Sum();
    public int CalculateZweier() => currentDiceValues.Where(d => d == 2).Sum();
    public int CalculateDreier() => currentDiceValues.Where(d => d == 3).Sum();
    public int CalculateVierer() => currentDiceValues.Where(d => d == 4).Sum();
    public int CalculateFuenfer() => currentDiceValues.Where(d => d == 5).Sum();
    public int CalculateSechser() => currentDiceValues.Where(d => d == 6).Sum();

    public int CalculateDreierpasch()
    {
        var groups = currentDiceValues.GroupBy(d => d);
        foreach (var group in groups)
        {
            if (group.Count() >= 3)
            {
                return currentDiceValues.Sum();
            }
        }
        return 0;
    }

    public int CalculateViererpasch()
    {
        var groups = currentDiceValues.GroupBy(d => d);
        foreach (var group in groups)
        {
            if (group.Count() >= 4)
            {
                return currentDiceValues.Sum();
            }
        }
        return 0;
    }

    public int CalculateFullHouse()
    {
        var groups = currentDiceValues.GroupBy(d => d).Select(g => g.Count()).OrderByDescending(c => c).ToArray();

        if (groups.Length == 2 && groups[0] == 3 && groups[1] == 2)
        {
            return 25;
        }
        return 0;
    }

    public int CalculateKleineStraße()
    {
        var uniqueDice = currentDiceValues.Distinct().OrderBy(d => d).ToArray();

        for (int i = 0; i <= uniqueDice.Length - 4; i++)
        {
            bool isStraight = true;
            for (int j = 1; j < 4; j++)
            {
                if (uniqueDice[i + j] != uniqueDice[i] + j)
                {
                    isStraight = false;
                    break;
                }
            }
            if (isStraight) return 30;
        }
        return 0;
    }

    public int CalculateGroßeStraße()
    {
        var uniqueDice = currentDiceValues.Distinct().OrderBy(d => d).ToArray();

        if (uniqueDice.Length == 5)
        {
            bool isStraight = true;
            for (int i = 1; i < 5; i++)
            {
                if (uniqueDice[i] != uniqueDice[0] + i)
                {
                    isStraight = false;
                    break;
                }
            }
            if (isStraight) return 40;
        }
        return 0;
    }

    public int CalculateKniffel()
    {
        if (currentDiceValues.All(d => d == currentDiceValues[0]))
        {
            return 50;
        }
        return 0;
    }

    public int CalculateChance() => currentDiceValues.Sum();

    public void CalculateTotal()
    {
        // Upper Section berechnen
        int upperTotal = 0;
        if (int.TryParse(einerInput.text, out int einer)) upperTotal += einer;
        if (int.TryParse(zweierInput.text, out int zweier)) upperTotal += zweier;
        if (int.TryParse(dreierInput.text, out int dreier)) upperTotal += dreier;
        if (int.TryParse(viererInput.text, out int vierer)) upperTotal += vierer;
        if (int.TryParse(fuenferInput.text, out int fuenfer)) upperTotal += fuenfer;
        if (int.TryParse(sechserInput.text, out int sechser)) upperTotal += sechser;

        gesamtUpperText.text = upperTotal.ToString();

        // Bonus berechnen
        int bonus = upperTotal >= 63 ? 35 : 0;
        bonusText.text = bonus.ToString();

        // Gesamtpunktzahl berechnen
        int total = upperTotal + bonus;

        // Lower Section hinzufügen
        if (int.TryParse(dreierpaschenInput.text, out int dreierpasch)) total += dreierpasch;
        if (int.TryParse(viererpaschenInput.text, out int viererpasch)) total += viererpasch;
        if (int.TryParse(fullHouseInput.text, out int fullHouse)) total += fullHouse;
        if (int.TryParse(kleineStraßeInput.text, out int kleineStraße)) total += kleineStraße;
        if (int.TryParse(großeStraßeInput.text, out int großeStraße)) total += großeStraße;
        if (int.TryParse(kniffelInput.text, out int kniffel)) total += kniffel;
        if (int.TryParse(chanceInput.text, out int chance)) total += chance;

        gesamtpunktzahlText.text = total.ToString();
    }

    public void ShowPossibleScores()
    {
        if (currentDiceValues.Count != 5) return;

        Debug.Log($"Mögliche Punkte für Wurf: [{string.Join(", ", currentDiceValues)}]");
        for (int i = 0; i < 13; i++)
        {
            if (!usedCategories[i])
            {
                int points = CalculatePointsForCategory(i);
                Debug.Log($"{GetCategoryName(i)}: {points} Punkte");
            }
        }
    }

    // ERSETZE deine EndGame() Methode im PointCalculator2D mit dieser:



void EndGame()
{
    Debug.Log($"🔍 User Check: {UserSessionManager.Instance?.LoggedInUsername ?? "NULL"}");
    Debug.Log("SPIEL BEENDET!");
    
    // Score über UserScoreManager speichern
    if (UserScoreManager.Instance != null)
    {
        UserScoreManager.Instance.SaveGameScore(this);
        Debug.Log("Score wurde gespeichert!");
    }
    else
    {
        Debug.LogWarning("UserScoreManager nicht gefunden - Score wird nicht gespeichert!");
    }
    
    // Optional: Game Over Modal anzeigen
    if (gameModalManager != null)
    {
        gameModalManager.OpenModal();
    }
    else
    {
        // Fallback: Console-Output mit Statistiken
        ShowGameOverInfoInConsole();
    }
}

// Hilfsmethode für Console-Output falls kein Modal vorhanden
private void ShowGameOverInfoInConsole()
{
    if (UserSessionManager.Instance != null && !string.IsNullOrEmpty(UserSessionManager.Instance.LoggedInUsername))
    {
        string currentUser = UserSessionManager.Instance.LoggedInUsername;
        int finalScore = 0;
        int.TryParse(gesamtpunktzahlText.text, out finalScore);
        
        Debug.Log($"🎉 Spiel beendet für {currentUser}!");
        Debug.Log($"📊 Finale Punkte: {finalScore}");
        
        if (UserScoreManager.Instance != null)
        {
            UserScoreData userData = UserScoreManager.Instance.GetUserScoreData(currentUser);
            
            Debug.Log($"🏆 Bester Score: {userData.bestScore}");
            Debug.Log($"🎮 Spiele gespielt: {userData.totalGamesPlayed}");
            Debug.Log($"📈 Durchschnitt: {userData.averageScore:F1}");
            
            if (finalScore >= userData.bestScore)
            {
                Debug.Log("🎊 NEUER PERSÖNLICHER REKORD! 🎊");
            }
        }
    }
    else
    {
        Debug.Log("Nicht eingeloggt - keine Statistiken verfügbar");
    }
}

    // Public Methoden für manuelle Kategorie-Auswahl (für UI Buttons)
    public void SelectEiner() { SelectCategory(0); }
    public void SelectZweier() { SelectCategory(1); }
    public void SelectDreier() { SelectCategory(2); }
    public void SelectVierer() { SelectCategory(3); }
    public void SelectFuenfer() { SelectCategory(4); }
    public void SelectSechser() { SelectCategory(5); }
    public void SelectDreierpasch() { SelectCategory(6); }
    public void SelectViererpasch() { SelectCategory(7); }
    public void SelectFullHouse() { SelectCategory(8); }
    public void SelectKleineStraße() { SelectCategory(9); }
    public void SelectGroßeStraße() { SelectCategory(10); }
    public void SelectKniffel() { SelectCategory(11); }
    public void SelectChance() { SelectCategory(12); }


    // DIESE METHODEN zu deinem bestehenden PointCalculator2D Script HINZUFÜGEN:
// Am Ende der Klasse, vor der letzten geschweiften Klammer }

public void ResetGame()
{
    // Alle verwendeten Kategorien zurücksetzen
    for (int i = 0; i < usedCategories.Length; i++)
    {
        usedCategories[i] = false;
    }
    
    filledCategories = 0;
    currentDiceValues.Clear();
    
    // Alle InputFields leeren
    ClearAllInputFields();
    
    // UI zurücksetzen
    UpdateCategoryButtons();
    
    // Neue Runde starten
    StartNewRound();
    
    Debug.Log("Spiel wurde zurückgesetzt!");
}

    void ClearAllInputFields()
    {
        // Upper Section
        if (einerInput != null) einerInput.text = "";
        if (zweierInput != null) zweierInput.text = "";
        if (dreierInput != null) dreierInput.text = "";
        if (viererInput != null) viererInput.text = "";
        if (fuenferInput != null) fuenferInput.text = "";
        if (sechserInput != null) sechserInput.text = "";
        if (gesamtUpperText != null) gesamtUpperText.text = "0";
        if (bonusText != null) bonusText.text = "0";

        // Lower Section
        if (dreierpaschenInput != null) dreierpaschenInput.text = "";
        if (viererpaschenInput != null) viererpaschenInput.text = "";
        if (fullHouseInput != null) fullHouseInput.text = "";
        if (kleineStraßeInput != null) kleineStraßeInput.text = "";
        if (großeStraßeInput != null) großeStraßeInput.text = "";
        if (kniffelInput != null) kniffelInput.text = "";
        if (chanceInput != null) chanceInput.text = "";

        // Total
        if (gesamtpunktzahlText != null) gesamtpunktzahlText.text = "0";

    }
}