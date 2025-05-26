using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PointCalculator pointCalculator;
    public DiceThrower diceThrower;
public void ExitGame()
    {
        Application.Quit();
    }

public void ResetGame()
    {
     if (diceThrower != null)
        diceThrower.ResetDiceAndTimer();

     if (pointCalculator != null)
         pointCalculator.ResetGame();
    }
}
