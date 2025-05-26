using UnityEngine;
using TMPro;

public class DiceHover : MonoBehaviour
{
    public TextMeshProUGUI hoverInfoText;
    private Dice dice;

    private void Start()
    {
        dice = GetComponent<Dice>();
    }

    private void OnMouseEnter()
    {
        if (hoverInfoText != null && dice != null)
        {
            int number = dice.GetNumberOnTopFace();
            hoverInfoText.text = "Face value: " + number;
        }
    }

    private void OnMouseExit()
    {
        if (hoverInfoText != null)
        {
            hoverInfoText.text = "";
        }
    }
}
