using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public GameObject dicePrefab; // UI-Prefab mit Image + Dice2DUI.cs
    public Sprite[] diceSprites; // 6 Sprites (Würfelseiten)
    public RectTransform container; // z. B. RollDiceFieldContainer

    private Vector2[] targetSlots = new Vector2[5] {
        new Vector2(-300, 0),
        new Vector2(-150, 0),
        new Vector2(   0, 0),
        new Vector2( 150, 0),
        new Vector2( 300, 0),
    };

    /*public void Start()
    {
        RollAllDices();
    }*/

    public void RollAllDices()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject); // Alle alten Würfel entfernen
        }
        for (int i = 0; i < targetSlots.Length; i++)
        {
            GameObject diceObj = Instantiate(dicePrefab, container);
            RectTransform rt = diceObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-600, 0); // Start ganz links außerhalb des Containers

            Dice2DUI dice = diceObj.GetComponent<Dice2DUI>();
            dice.diceSides = diceSprites;
            dice.targetPosition = targetSlots[i];
            dice.StartRoll();
        }
    }
}