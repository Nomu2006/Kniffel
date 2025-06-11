using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    public DiceThrower diceThrower;
    public TMP_Text timeValueLabel;
    public TMP_Text tryValueLabel;

    private float totalTime = 90f;
    private int maxTries = 3;
    private int currentTry = 0;
    private float remainingTime;
    public bool roundActive { get; private set; } = false;
    public bool waitingForScore = false;
    private bool hasScoredThisRound = false;




    void Start()
    {
        UpdateUI(); 
    }

    void Update()
    {
        if (!roundActive) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndRound();
        }

        UpdateUI();
    }

    public void RegisterRoll()
    {
        if (!roundActive) return;

        currentTry++;
        UpdateUI();

        if (currentTry >= maxTries)
        {
            EndRound();
        }
    }

    private void StartNewRound()
    {
        currentTry = 0;
        remainingTime = totalTime;
        roundActive = true;

        diceThrower.ResetDiceAndTimer();
        UpdateUI();
    }

    public void StartRound()
    {
        if (roundActive) return;

        currentTry = 0;
        remainingTime = totalTime;
        roundActive = true;
        waitingForScore = false;
        hasScoredThisRound = false; // <--- NEU

        diceThrower.enabled = true;
        diceThrower.ResetDiceAndTimer();
        UpdateUI();
    }




    private void EndRound()
    {
        roundActive = false;
        waitingForScore = true; // Spieler muss jetzt Punkte vergeben
        diceThrower.enabled = false;
        Debug.Log("Runde beendet! Bitte Punkte wählen.");
    }

    
    private string TryToText(int tryNumber)
    {
        if (tryNumber == 0)
            return "ROLL DICE";
        switch (tryNumber)
        {
            case 1: return "FIRST TRY";
            case 2: return "SECOND TRY";
            case 3: return "THIRD TRY";
            default: return $"{tryNumber}/{maxTries}";
        }
    }
    
    public void ConfirmScoring()
    {
        if (hasScoredThisRound)
        {
            Debug.Log("Du hast diese Runde bereits Punkte vergeben.");
            return;
        }

        hasScoredThisRound = true;
        waitingForScore = false;
        StartRound();
    }



    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timeValueLabel.text = $"{minutes:00}:{seconds:00}";
        tryValueLabel.text = TryToText(currentTry);
    }
}
