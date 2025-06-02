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

        float elapsed = 0f;
        float spriteTimer = 0f;
        float currentInterval = rollInterval;
        Vector2 startPos = rt.anchoredPosition;

        while (elapsed < rollDuration)
        {
            // Bewegung
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsed / rollDuration);

            // Sprite-Wechsel alle X Sekunden
            spriteTimer += Time.deltaTime;
            if (spriteTimer >= currentInterval)
            {
                int r = Random.Range(0, 6);
                img.sprite = diceSides[r];
                spriteTimer = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null; // ← kein harter Wait mehr!
        }

        // Endgültiges Ergebnis anzeigen
        CurrentValue = Random.Range(1, 7);
        img.sprite = diceSides[CurrentValue - 1];
        rt.anchoredPosition = targetPosition;
    }
}