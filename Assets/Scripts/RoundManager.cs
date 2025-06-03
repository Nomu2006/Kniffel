using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    public DiceThrower diceThrower;
    public TMP_Text timeValueLabel;
    public TMP_Text tryValueLabel;

    private float totalTime = 90f; // 1 Minute 30 Sekunden
    private int maxTries = 3;
    private int currentTry = 0;
    private float remainingTime;
    public bool roundActive { get; private set; } = false;


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

    diceThrower.ResetDiceAndTimer();
    UpdateUI();
}


    private void EndRound()
    {
        roundActive = false;
        diceThrower.enabled = false;
        Debug.Log("Runde beendet!");
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timeValueLabel.text = $"{minutes:00}:{seconds:00}";
        tryValueLabel.text = $"{currentTry}/{maxTries}";
    }
}
