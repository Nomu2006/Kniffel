using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class PointCalculator : MonoBehaviour
{
    public DiceThrower diceThrower;

    public TextMeshProUGUI onesText, twosText, threesText, foursText, fivesText, sixesText, 
                           threeOfAKindText, fourOfAKindText, 
                           fullHouseText, smallStraightText, largeStraightText, yahtzeeText, chanceText,
                           bonusText, totalUpperText, totalLowerText, totalFinalText, HighscoreText;

    private int points1, points2, points3, points4, points5, points6,
                threeOfAKind, fourOfAKind, fullHouse, smallStraight, largeStraight, yahtzee, chance,
                bonus, totalUpper, totalLower, highscore;
    private int ComputeSum(List<Dice> dice, int targetVal)
    {
        return dice.Where(d => d.GetNumberOnTopFace() == targetVal).Sum(d => targetVal);
    }

    private int GetValueFromText(TextMeshProUGUI field)
    {
        if (field == null || string.IsNullOrWhiteSpace(field.text)) return 0;
        return int.TryParse(field.text, out int result) ? result : 0;
    }
    private void Start()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        ChangeText(HighscoreText, highscore);
    }

    private void ChangeText(TextMeshProUGUI target, int value)
    {
        if (target != null) target.text = value.ToString();
    }

    private void ChangeTextLong(TextMeshProUGUI target, string label, int value)
    {
        if (target != null) target.text = label + value.ToString();
    }

    private TextMeshProUGUI GetTextField(int number)
    {
        return number switch
        {
            1 => onesText,
            2 => twosText,
            3 => threesText,
            4 => foursText,
            5 => fivesText,
            6 => sixesText,
            _ => null
        };
    }

    private void UpdateUpperTotal()
    {
        int upperTotal = GetValueFromText(onesText) + GetValueFromText(twosText) +
                         GetValueFromText(threesText) + GetValueFromText(foursText) +
                         GetValueFromText(fivesText) + GetValueFromText(sixesText);

        ChangeText(totalUpperText, upperTotal);

        int bonus = upperTotal >= 63 ? 35 : 0;
        ChangeText(bonusText, bonus);

        int totalLower = upperTotal + bonus;
        ChangeText(totalLowerText, totalLower);

        UpdateFinalTotal();
    }

    private void UpdateFinalTotal()
    {
        int upperTotal = GetValueFromText(totalUpperText);
        int bonus = GetValueFromText(bonusText);

        int lowerTotal = GetValueFromText(threeOfAKindText) + GetValueFromText(fourOfAKindText) +
                         GetValueFromText(fullHouseText) + GetValueFromText(smallStraightText) +
                         GetValueFromText(largeStraightText) + GetValueFromText(yahtzeeText) +
                         GetValueFromText(chanceText);

        int finalTotal = upperTotal + bonus + lowerTotal;
        ChangeText(totalFinalText, finalTotal);

        int Highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (finalTotal > Highscore)
        {
            PlayerPrefs.SetInt("Highscore", finalTotal);
            PlayerPrefs.Save();
            Highscore = finalTotal;
            ChangeText(HighscoreText, Highscore);
        }
    }


    public void ScoreThreeOfAKind() => ScoreOfAKind(3, threeOfAKindText);
    public void ScoreFourOfAKind() => ScoreOfAKind(4, fourOfAKindText);

    public void ScoreNumber(int number)
    {
        int points = ComputeSum(diceThrower.spawnedDice, number);
        ChangeTextLong(GetTextField(number), "", points);
        UpdateUpperTotal();
    }

    private void ScoreOfAKind(int count, TextMeshProUGUI textField)
    {
        var groups = diceThrower.spawnedDice.GroupBy(d => d.GetNumberOnTopFace());
        int sum = groups.Any(g => g.Count() >= count) ? diceThrower.spawnedDice.Sum(d => d.GetNumberOnTopFace()) : 0;
        ChangeTextLong(textField, "", sum);
        UpdateFinalTotal();
    }

    public void ScoreFullHouse()
    {
        var groups = diceThrower.spawnedDice.GroupBy(d => d.GetNumberOnTopFace()).Select(g => g.Count()).ToList();
        int score = (groups.Contains(3) && groups.Contains(2)) ? 25 : 0;
        fullHouse = score;
        ChangeTextLong(fullHouseText, "", fullHouse);
        UpdateFinalTotal();
    }

    public void ScoreSmallStraight()
    {
        var unique = diceThrower.spawnedDice.Select(d => d.GetNumberOnTopFace()).Distinct().OrderBy(n => n).ToList();
        var straights = new List<List<int>> {
            new() { 1, 2, 3, 4 },
            new() { 2, 3, 4, 5 },
            new() { 3, 4, 5, 6 }
        };
        int score = straights.Any(s => s.All(unique.Contains)) ? 30 : 0;
        smallStraight = score;
        ChangeTextLong(smallStraightText, "", smallStraight);
        UpdateFinalTotal();
    }

    public void ScoreLargeStraight()
    {
        var unique = diceThrower.spawnedDice.Select(d => d.GetNumberOnTopFace()).Distinct().OrderBy(n => n).ToList();
        var straights = new List<List<int>> {
            new() { 1, 2, 3, 4, 5 },
            new() { 2, 3, 4, 5, 6 }
        };
        int score = straights.Any(s => s.All(unique.Contains)) ? 40 : 0;
        largeStraight = score;
        ChangeTextLong(largeStraightText, "", largeStraight);
        UpdateFinalTotal();
    }

    public void ScoreYahtzee()
    {
        int val = diceThrower.spawnedDice[0].GetNumberOnTopFace();
        bool allSame = diceThrower.spawnedDice.All(d => d.GetNumberOnTopFace() == val);
        int score = allSame ? 50 : 0;
        yahtzee = score;
        ChangeTextLong(yahtzeeText, "", yahtzee);
        UpdateFinalTotal();
    }


    public void ScoreChance()
    {
        int score = diceThrower.spawnedDice.Sum(d => d.GetNumberOnTopFace());
        chance = score;
        ChangeTextLong(chanceText, "", chance);
        UpdateFinalTotal();
    }
    public void ResetHighscore()
    {
        PlayerPrefs.DeleteKey("Highscore");
        ChangeText(HighscoreText, 0);
    }



    public void ResetGame()
    {
        points1 = points2 = points3 = points4 = points5 = points6 = 0;
        threeOfAKind = fourOfAKind = fullHouse = smallStraight = largeStraight = yahtzee = chance = 0;
        bonus = totalUpper = totalLower = 0;

        onesText.text = points1.ToString();
        twosText.text = points2.ToString();
        threesText.text = points3.ToString();
        foursText.text = points4.ToString();
        fivesText.text = points5.ToString();
        sixesText.text = points6.ToString();

        threeOfAKindText.text = threeOfAKind.ToString();
        fourOfAKindText.text = fourOfAKind.ToString();
        fullHouseText.text = fullHouse.ToString();
        smallStraightText.text = smallStraight.ToString();
        largeStraightText.text = largeStraight.ToString();
        yahtzeeText.text = yahtzee.ToString();
        chanceText.text = chance.ToString();

        bonusText.text = bonus.ToString();
        totalUpperText.text = totalUpper.ToString();
        totalLowerText.text = totalLower.ToString();
        totalFinalText.text = (totalUpper + totalLower + bonus).ToString();
    }
}
