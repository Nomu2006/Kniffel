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

    // Tracking welche Kategorien bereits verwendet wurden
    private bool[] usedCategories = new bool[13]; // 6 upper + 7 lower
    private int filledCategories = 0;

    // Aktuelle Würfelwerte
    private List<int> currentDiceValues = new List<int>();

    void Start()
    {
        // Button Events setup
        SetupRowButtons();

        // Alle InputFields zu Beginn read-only
        SetAllFieldsReadOnly();
        
        // Neue Runde starten
        StartNewRound();
    }

    void Update()
    {
        // Würfelwerte aktualisieren wenn Round beendet oder nach jedem Wurf
        if (roundManager != null && !diceThrower.IsRolling())
        {
            UpdateCurrentDiceValues();
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

        // Prüfen ob Runde aktiv ist
        if (roundManager == null || roundManager.IsRoundActive())
        {
            Debug.Log("Runde noch aktiv - beende erst die Würfe!");
            return;
        }

        // Prüfen ob Würfelwerte vorhanden
        if (currentDiceValues.Count != 5)
        {
            Debug.Log("Keine gültigen Würfelwerte vorhanden!");
            return;
        }

        // Punkte berechnen und eintragen
        int points = CalculatePointsForCategory(categoryIndex);
        SetCategoryPoints(categoryIndex, points);
        
        // Kategorie als verwendet markieren
        usedCategories[categoryIndex] = true;
        filledCategories++;
        
        // UI aktualisieren
        UpdateCategoryButtons();
        CalculateTotal();
        
        Debug.Log($"Kategorie {GetCategoryName(categoryIndex)} gewählt: {points} Punkte");
        
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

    void UpdateCategoryButtons()
    {
        // Row Buttons aktivieren/deaktivieren basierend auf Verfügbarkeit
        Button[] rowButtons = {einerRowButton, zweierRowButton, dreierRowButton, viererRowButton, 
                              fuenferRowButton, sechserRowButton, dreierpaschenRowButton, 
                              viererpaschenRowButton, fullHouseRowButton, kleineStraßeRowButton, 
                              großeStraßeRowButton, kniffelRowButton, chanceRowButton};

        for (int i = 0; i < rowButtons.Length; i++)
        {
            if (rowButtons[i] != null)
            {
                // Button deaktivieren wenn bereits verwendet
                rowButtons[i].interactable = !usedCategories[i];
                
                // Visuelles Feedback für verwendete Kategorien
                ColorBlock colors = rowButtons[i].colors;
                if (usedCategories[i])
                {
                    colors.normalColor = Color.gray;
                    colors.disabledColor = Color.gray;
                }
                else
                {
                    colors.normalColor = Color.white;
                    colors.highlightedColor = Color.yellow;
                }
                rowButtons[i].colors = colors;
            }
        }
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

    // Berechnungsmethoden (wie vorher)
    public int CalculateEiner()
    {
        return currentDiceValues.Where(d => d == 1).Sum();
    }

    public int CalculateZweier()
    {
        return currentDiceValues.Where(d => d == 2).Sum();
    }

    public int CalculateDreier()
    {
        return currentDiceValues.Where(d => d == 3).Sum();
    }

    public int CalculateVierer()
    {
        return currentDiceValues.Where(d => d == 4).Sum();
    }

    public int CalculateFuenfer()
    {
        return currentDiceValues.Where(d => d == 5).Sum();
    }

    public int CalculateSechser()
    {
        return currentDiceValues.Where(d => d == 6).Sum();
    }

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

    public int CalculateChance()
    {
        return currentDiceValues.Sum();
    }

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

    void EndGame()
    {
        Debug.Log("SPIEL BEENDET!");
        Debug.Log($"Endpunktzahl: {gesamtpunktzahlText.text}");
        
        // Hier könntest du ein Game Over UI anzeigen
        // Oder Highscore speichern, etc.
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
}