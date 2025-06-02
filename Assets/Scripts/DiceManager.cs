using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class DiceManager : MonoBehaviour
{
    public GameObject dicePrefab; // UI-Prefab mit Image + Dice2DUI.cs
    public Sprite[] diceSprites; // 6 Sprites (Würfelseiten)
    public RectTransform container; // z. B. RollDiceFieldContainer

    /*private Vector2[] targetSlots = new Vector2[5] {
        new Vector2(-300, 0),
        new Vector2(-150, 0),
        new Vector2(   0, 0),
        new Vector2( 150, 0),
        new Vector2( 300, 0),
    };*/
    public int diceCount = 5;
    public float fieldWidth = 600f;
    public float fieldHeight = 200f;


    /*public void Start()
    {
        RollAllDices();
    }*/

    public void RollAllDices()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        List<Vector2> usedPositions = new List<Vector2>();
        float minDistance = 120f; // Abstand zwischen Würfeln (je nach Größe des Images)

        for (int i = 0; i < diceCount; i++)
        {
            Vector2 target;

            // Versuche mehrfach, eine freie Position zu finden
            int attempts = 0;
            do
            {
                float randomX = Random.Range(-fieldWidth / 2f, fieldWidth / 2f);
                float randomY = Random.Range(-fieldHeight / 2f, fieldHeight / 2f);
                target = new Vector2(randomX, randomY);
                attempts++;
            } while (usedPositions.Exists(pos => Vector2.Distance(pos, target) < minDistance) && attempts < 50);

            usedPositions.Add(target);

            GameObject diceObj = Instantiate(dicePrefab, container);
            RectTransform rt = diceObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-800, 0); // Start ganz links

            Dice2DUI dice = diceObj.GetComponent<Dice2DUI>();
            dice.diceSides = diceSprites;
            dice.targetPosition = target;
            dice.StartRoll();
        }
    }
}