using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice2DThrower : MonoBehaviour
{
    [Header("Dice Settings")]
    public Sprite[] diceSprites; // Array für die 6 Würfel-Sprites (1-6)
    public GameObject dicePrefab; // Prefab für einen Würfel (mit Image Component)
    public Transform rollDiceFieldContainer; // Container wo die Würfel landen
    public Button rollButton; // Der Roll Button

    [Header("Animation Settings")]
    public int numberOfDice = 5; // Anzahl der Würfel (für Kniffel)
    public float animationDuration = 1.0f; // Dauer der Animation

    private List<GameObject> currentDice = new List<GameObject>(); // Aktuelle Würfel
    private List<int> diceResults = new List<int>(); // Ergebnisse der Würfel
    private List<Vector2> targetPositions = new List<Vector2>(); // Gespeicherte Zielpositionen
    private bool isRolling = false; // Verhindert mehrfaches Würfeln

    void Start()
    {
        // Button Event hinzufügen (wird später vom Round2DManager überschrieben)
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(RollDice);
        }
    }
    
    void Update()
    {
        // Leertaste zum Würfeln über Round2DManager
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            Round2DManager roundManager = FindFirstObjectByType<Round2DManager>();
            if (roundManager != null)
            {
                roundManager.HandleRollButton();
            }
            else
            {
                RollDice(); // Fallback
            }
        }
    }

    public void RollDice()
    {
        if (isRolling) return;

        Debug.Log("RollDice called!");
        
        // Prüfen ob Round2DManager existiert und ob es nach dem ersten Wurf ist
        Round2DManager roundManager = FindFirstObjectByType<Round2DManager>();
        bool hasHeldDice = roundManager != null && roundManager.GetHeldDice().Count > 0;
        
        if (hasHeldDice)
        {
            Debug.Log("Rolling only non-held dice");
            // Nur nicht-gehaltene Würfel würfeln (2. oder 3. Wurf)
            List<GameObject> diceToRoll = GetDiceNotInHold(roundManager);
            if (diceToRoll.Count > 0)
            {
                StartCoroutine(RollOnlyFieldDice(diceToRoll));
            }
            else
            {
                Debug.Log("No dice to roll - all are held!");
            }
        }
        else
        {
            Debug.Log("Rolling all dice (first roll)");
            // Alle Würfel würfeln (erster Wurf)
            StartCoroutine(RollDiceAnimation());
        }
    }

    IEnumerator RollDiceAnimation()
    {
        isRolling = true;

        // Button deaktivieren während Animation
        if (rollButton != null)
            rollButton.interactable = false;

        // Alte Würfel entfernen
        ClearDice();

        // Neue Würfel erstellen
        CreateDice();

        // Animation für jeden Würfel
        List<Coroutine> animations = new List<Coroutine>();
        for (int i = 0; i < currentDice.Count; i++)
        {
            animations.Add(StartCoroutine(AnimateDice(currentDice[i], i)));
        }

        // Warten bis alle Animationen fertig sind
        foreach (var animation in animations)
        {
            yield return animation;
        }

        // Button wieder aktivieren
        if (rollButton != null)
            rollButton.interactable = true;

        isRolling = false;

        // Debug Output
        Debug.Log("Dice Results: " + string.Join(", ", diceResults));
    }

    void CreateDice()
    {
        // Container Dimensionen
        RectTransform containerRect = rollDiceFieldContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Würfel sind 100x100, also brauchen wir mindestens 150px Abstand
        float diceSize = 100f;
        float minDistance = 150f; // Mindestabstand zwischen Würfeln

        // ERST alle Positionen berechnen
        targetPositions.Clear();
        for (int i = 0; i < numberOfDice; i++)
        {
            Vector2 targetPos = FindValidPosition(containerWidth, containerHeight, targetPositions, minDistance);
            targetPositions.Add(targetPos);
        }

        // DANN die Würfel erstellen
        for (int i = 0; i < numberOfDice; i++)
        {
            // Würfel erstellen
            GameObject dice = Instantiate(dicePrefab, rollDiceFieldContainer);
            currentDice.Add(dice);

            // Zufällige Würfelzahl (1-6)
            int randomValue = Random.Range(1, 7);
            diceResults.Add(randomValue);

            // Entsprechendes Sprite zuweisen
            Image diceImage = dice.GetComponent<Image>();
            if (diceImage != null && diceSprites.Length >= 6)
            {
                diceImage.sprite = diceSprites[randomValue - 1];
            }

            // Physics Komponenten entfernen falls vorhanden
            Rigidbody2D rb = dice.GetComponent<Rigidbody2D>();
            if (rb != null) DestroyImmediate(rb);

            Collider2D collider = dice.GetComponent<Collider2D>();
            if (collider != null) DestroyImmediate(collider);

            // RectTransform Setup
            RectTransform diceRect = dice.GetComponent<RectTransform>();
            diceRect.anchorMin = new Vector2(0.5f, 0.5f);
            diceRect.anchorMax = new Vector2(0.5f, 0.5f);
            diceRect.pivot = new Vector2(0.5f, 0.5f);
            diceRect.sizeDelta = new Vector2(diceSize, diceSize);

            // Click Event für Würfel hinzufügen
            Button diceButton = dice.GetComponent<Button>();
            if (diceButton == null)
            {
                diceButton = dice.AddComponent<Button>();
            }

            // Click Event - Würfel zwischen Hold und Field Container bewegen
            int diceIndex = i; // Lokale Kopie für Closure
            diceButton.onClick.AddListener(() => {
                Round2DManager roundManager = FindFirstObjectByType<Round2DManager>();
                if (roundManager != null)
                {
                    // Prüfen ob Würfel im Hold Container ist
                    if (dice.transform.parent == roundManager.holdDiceContainer)
                    {
                        roundManager.MoveDiceToField(dice);
                    }
                    else
                    {
                        roundManager.MoveDiceToHold(dice);
                    }
                }
            });

            // Startposition links außerhalb (mit der entsprechenden Y-Position)
            Vector2 startPos = new Vector2(-containerWidth / 2 - 150f, targetPositions[i].y + Random.Range(-30f, 30f));
            diceRect.anchoredPosition = startPos;
        }
    }

    Vector2 FindValidPosition(float containerWidth, float containerHeight, List<Vector2> existingPositions, float minDistance)
    {
        Vector2 newPosition;
        int maxAttempts = 100;
        float margin = 80f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Zufällige Position generieren
            newPosition = new Vector2(
                Random.Range(-containerWidth / 2 + margin, containerWidth / 2 - margin),
                Random.Range(-containerHeight / 2 + margin, containerHeight / 2 - margin)
            );

            // Prüfen ob Position genug Abstand zu allen anderen hat
            bool validPosition = true;
            foreach (Vector2 existingPos in existingPositions)
            {
                if (Vector2.Distance(newPosition, existingPos) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition)
            {
                return newPosition;
            }
        }

        // Fallback: Position trotzdem zurückgeben (verhindert Endlosschleife)
        return new Vector2(
            Random.Range(-containerWidth / 2 + margin, containerWidth / 2 - margin),
            Random.Range(-containerHeight / 2 + margin, containerHeight / 2 - margin)
        );
    }

    IEnumerator AnimateDice(GameObject dice, int index)
    {
        RectTransform diceRect = dice.GetComponent<RectTransform>();
        Vector2 startPos = diceRect.anchoredPosition;

        // Zielposition aus der gespeicherten Liste holen
        Vector2 targetPos = targetPositions[index];

        float elapsed = 0f;
        float delay = index * 0.2f; // Verzögerung zwischen Würfeln

        // Warten auf Verzögerung
        yield return new WaitForSeconds(delay);

        // Sprite Animation während Wurf
        Coroutine spriteAnimation = StartCoroutine(AnimateRandomSprites(dice));

        // Einfache, glatte Animation
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // Smooth Animation ohne Wackeln
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, smoothProgress);

            // Einfacher Bogen ohne komplizierte Berechnungen
            currentPos.y += Mathf.Sin(smoothProgress * Mathf.PI) * 40f;

            diceRect.anchoredPosition = currentPos;

            yield return null;
        }

        // Sprite Animation stoppen
        StopCoroutine(spriteAnimation);

        // Finales Sprite setzen
        Image diceImage = dice.GetComponent<Image>();
        int finalValue = diceResults[index];
        if (diceImage != null && diceSprites.Length >= 6)
        {
            diceImage.sprite = diceSprites[finalValue - 1];
        }

        // Endposition exakt setzen
        diceRect.anchoredPosition = targetPos;
    }

    IEnumerator AnimateRandomSprites(GameObject dice)
    {
        Image diceImage = dice.GetComponent<Image>();

        while (true)
        {
            if (diceImage != null && diceSprites.Length >= 6)
            {
                int randomSprite = Random.Range(0, 6);
                diceImage.sprite = diceSprites[randomSprite];
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void ClearDice()
    {
        // NUR Würfel zerstören die NICHT im Hold Container sind
        Round2DManager roundManager = FindFirstObjectByType<Round2DManager>();
        
        List<GameObject> diceToDestroy = new List<GameObject>();
        
        for (int i = currentDice.Count - 1; i >= 0; i--)
        {
            GameObject dice = currentDice[i];
            if (dice != null)
            {
                // Prüfen ob Würfel im Hold Container ist
                bool isInHold = roundManager != null && 
                               dice.transform.parent == roundManager.holdDiceContainer;
                
                if (!isInHold)
                {
                    diceToDestroy.Add(dice);
                    currentDice.RemoveAt(i);
                    if (i < diceResults.Count)
                        diceResults.RemoveAt(i);
                }
            }
        }
        
        // Nur nicht-gehaltene Würfel zerstören
        foreach (GameObject dice in diceToDestroy)
        {
            Destroy(dice);
        }
        
        targetPositions.Clear();
    }

    // Getter für die Würfelergebnisse
    public List<int> GetDiceResults()
    {
        return new List<int>(diceResults);
    }

    // Check ob gerade gewürfelt wird
    public bool IsRolling()
    {
        return isRolling;
    }

    // Getter für die aktuellen Würfel GameObjects
    public GameObject[] GetCurrentDiceObjects()
    {
        return currentDice.ToArray();
    }

    // Hilfsmethoden für Kniffel-Logik
    List<GameObject> GetDiceNotInHold(Round2DManager roundManager)
    {
        List<GameObject> diceNotInHold = new List<GameObject>();
        
        foreach (GameObject dice in currentDice)
        {
            if (dice != null && dice.transform.parent != roundManager.holdDiceContainer)
            {
                diceNotInHold.Add(dice);
            }
        }
        
        return diceNotInHold;
    }
    
    IEnumerator RollOnlyFieldDice(List<GameObject> diceToRoll)
    {
        isRolling = true;

        // Button deaktivieren während Animation
        if (rollButton != null)
            rollButton.interactable = false;

        // Neue Positionen für Würfel im Feld berechnen
        RectTransform containerRect = rollDiceFieldContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        float minDistance = 150f;
        
        // Bestehende Positionen von gehaltenen Würfeln sammeln
        List<Vector2> existingPositions = new List<Vector2>();
        Round2DManager roundManager = FindFirstObjectByType<Round2DManager>();
        
        if (roundManager != null)
        {
            foreach (GameObject heldDice in roundManager.GetHeldDice())
            {
                if (heldDice != null)
                {
                    RectTransform heldRect = heldDice.GetComponent<RectTransform>();
                    existingPositions.Add(heldRect.anchoredPosition);
                }
            }
        }

        // Neue Positionen für Würfel im Feld finden
        List<Vector2> newTargetPositions = new List<Vector2>();
        foreach (GameObject dice in diceToRoll)
        {
            Vector2 newPos = FindValidPosition(containerWidth, containerHeight, 
                                               existingPositions, minDistance);
            newTargetPositions.Add(newPos);
            existingPositions.Add(newPos); // Für nächste Iteration
            
            // Neue Würfelzahl generieren
            int diceIndex = currentDice.IndexOf(dice);
            if (diceIndex >= 0)
            {
                int newValue = Random.Range(1, 7);
                diceResults[diceIndex] = newValue;
            }
        }

        // Animation für Würfel im Feld
        List<Coroutine> animations = new List<Coroutine>();
        for (int i = 0; i < diceToRoll.Count; i++)
        {
            animations.Add(StartCoroutine(AnimateFieldDice(diceToRoll[i], newTargetPositions[i], i)));
        }

        // Warten bis alle Animationen fertig sind
        foreach (var animation in animations)
        {
            yield return animation;
        }

        // Button wieder aktivieren
        if (rollButton != null)
            rollButton.interactable = true;

        isRolling = false;

        Debug.Log("Field Dice Results: " + string.Join(", ", diceResults));
    }
    
    IEnumerator AnimateFieldDice(GameObject dice, Vector2 targetPos, int index)
    {
        RectTransform diceRect = dice.GetComponent<RectTransform>();
        Vector2 startPos = diceRect.anchoredPosition;

        float elapsed = 0f;
        float delay = index * 0.1f;

        // Warten auf Verzögerung
        yield return new WaitForSeconds(delay);

        // Sprite Animation während Wurf
        Coroutine spriteAnimation = StartCoroutine(AnimateRandomSprites(dice));

        // Animation
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, smoothProgress);
            currentPos.y += Mathf.Sin(smoothProgress * Mathf.PI) * 30f;

            diceRect.anchoredPosition = currentPos;

            yield return null;
        }

        // Sprite Animation stoppen
        StopCoroutine(spriteAnimation);

        // Finales Sprite setzen
        Image diceImage = dice.GetComponent<Image>();
        int diceIndex = currentDice.IndexOf(dice);
        if (diceIndex >= 0 && diceImage != null && diceSprites.Length >= 6)
        {
            int finalValue = diceResults[diceIndex];
            diceImage.sprite = diceSprites[finalValue - 1];
        }

        // Endposition setzen
        diceRect.anchoredPosition = targetPos;
    }
}