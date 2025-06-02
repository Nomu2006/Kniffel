using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice2DUI : MonoBehaviour
{
    public Sprite[] diceSides; // 6 Seiten von 1 bis 6
    public Vector2 targetPosition; // Zielposition im Container
    public float rollDuration = 1.2f;
    public float rollInterval = 0.1f;

    private Image img;
    private RectTransform rt;

    public int CurrentValue { get; private set; }

    private void Awake()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    public void StartRoll()
    {
        StartCoroutine(RollAnimation());
    }

    private IEnumerator RollAnimation()
    {
        if (diceSides == null || diceSides.Length < 6)
        {
            Debug.LogError("diceSides ist nicht korrekt belegt!");
            yield break;
        }

        float time = 0f;
        Vector2 startPos = rt.anchoredPosition;

        while (time < rollDuration)
        {
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPosition, time / rollDuration);
            int r = Random.Range(0, 6);
            img.sprite = diceSides[r];
            time += rollInterval;
            yield return new WaitForSeconds(rollInterval);
        }

        CurrentValue = Random.Range(1, 7);
        img.sprite = diceSides[CurrentValue - 1];
        rt.anchoredPosition = targetPosition;
    }
}