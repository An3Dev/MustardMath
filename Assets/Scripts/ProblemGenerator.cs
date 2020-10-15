using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProblemGenerator : MonoBehaviour
{
    public TextMeshProUGUI problemText;

    public TextMeshProUGUI[] levelText;

    public static int[] levels;
    public TextMeshProUGUI[] possibleSolutionsText;

    List<float> numberList = new List<float>();

    float solution;
    string operationString;
    bool giveNegativeSolutions = false;
    public static bool[] operations = new bool[] { true, true, true, true };

    public Color greyBtnColor;
    public Color[] buttonColors;
    public GameObject startBtn;
    public GameObject selectOperationsMessage;

    int[] winningStreaks = new int[] { 0, 0, 0, 0 };
    int[] losingStreaks = new int[] { 0, 0, 0, 0 };

    public Animator canvasAnim;

    public Image[] operationButtons;
    string levelsPrefsKey;

    // the 4 arrays below hold the max values that the student will be given depending on their level.
    int[] additionRangeOfNums = new int[] { 3, 5, 10, 20, 30, 40, 50, 60 };
    int[] minAdditionNums = new     int[] { 0, 1, 1, 5, 5, 10, 10, 10 };

    int[] subtractionRangeOfNums = new int[] { 5, 7, 10, 20, 30, 40, 50, 60 };
    int[] minSubtractionNums = new int[] { 0, 1, 2, 3, 5, 10, 10, 10};

    int[] multiplicationRangeOfNums = new int[] { 3, 5, 7, 10, 12 };
    int[] minMultiplicationNums = new int[] { 0, 1, 2, 3, 5, 7 };

    int[] divisionRangeOfNums = new int[] { 3, 5, 7, 10, 12 };
    int[] minDivisionNums = new int[] { 0, 1, 2, 3, 5, 5};
    public Transform solutionTransform;

    String streakKey = "StreakKey";
    String losingStreakKey = "LosingStreakKey";


    int randomOperation = -1;
    int streaksToProgress = 5;
    int losingStreaksToDemote = 3;

    private void Start()
    {
        levels = GetArrayFromPrefs(levelsPrefsKey);
        winningStreaks = GetArrayFromPrefs(streakKey);
        losingStreaks = GetArrayFromPrefs(losingStreakKey);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
           for(int i = 0; i < levels.Length; i++)
            {
                levelText[i].text = "Level " + (levels[i] + 1);
            }
        }

        if (operationButtons == null || operationButtons.Length == 0)
        {
            return;
        }

        for (int i = 0; i < operationButtons.Length; i++)
        {
            // if this operation is not selected
            if (!operations[i])
            {
                operationButtons[i].color = greyBtnColor;
            } else
            {
                operationButtons[i].color = buttonColors[i]; // give color to each button
            }
        }

        foreach (bool operation in operations)
        {
            if (operation)
            {
                startBtn.SetActive(true);
                selectOperationsMessage.SetActive(false);
                return;
            }
        }
        // disable the start button because no operations are selected
        startBtn.SetActive(false);
        selectOperationsMessage.SetActive(true);
    }

    #region data storage

    //Formats array from array to string to be stored as string in Player Prefs.
    void SaveArrayAsPref(string key, int[] array)
    {
        string thisString = "";

        //separates
        for (int i = 0; i < array.Length; i++)
        {
            if (i == 0)
            {
                thisString += array[i].ToString();
            }
            else
            {
                thisString += "," + array[i].ToString();
            }
        }
        PlayerPrefs.SetString(key, thisString);
    }

    // Gets string data that was formatted by GetArrayFromPrefs method and stored in player prefs. Reformats it to an array.
    int[] GetArrayFromPrefs(string key)
    {
        // Get data
        string data = PlayerPrefs.GetString(key, "0,0,0,0");

        string[] array = data.Split(',');

        int[] intArray = new int[4];

        for (int i = 0; i < array.Length; i++)
        {
            intArray[i] = int.Parse(array[i]);
        }

        return intArray;
    }

#endregion data storage

    // Called by buttons in menu
    public void AddOperation(int index)
    {
        // if this operation is disabled, enable it
        if (!operations[index])
        {
            operations[index] = true;
            operationButtons[index].color = buttonColors[index];
        } else //disable the operation
        {
            operations[index] = false;
            operationButtons[index].color = greyBtnColor;
        }

        foreach(bool operation in operations)
        {
            if (operation)
            {
                startBtn.SetActive(true);
                selectOperationsMessage.SetActive(false);

                return;
            }
        }
        // disable the start button because no operations are selected
        startBtn.SetActive(false);
        selectOperationsMessage.SetActive(true);
    }

    // called by start button
    public void StartGame(int operation)
    {
        SceneManager.LoadScene(1);
    }

    public void PlayCorrectAnswerAnimation()
    {
        canvasAnim.SetTrigger("CorrectAnswer");
    }

    public void PlayWrongAnswerAnimation()
    {
        canvasAnim.SetTrigger("WrongAnswer");
    }
    public void FadeOutText()
    {
        canvasAnim.SetTrigger("FadeOutText");
    }

    public void UpdateText()
    {
        problemText.text = numberList[0] + " " + operationString + " " + numberList[1] + " = " + solution;
    }

    public void AddToStreak()
    {
        winningStreaks[randomOperation]++;
        losingStreaks[randomOperation] = 0;

        if(winningStreaks[randomOperation] >= streaksToProgress)
        {
            winningStreaks[randomOperation] = 0;
            levels[randomOperation]++;
        }

        SaveArrayAsPref(streakKey, winningStreaks);
        SaveArrayAsPref(levelsPrefsKey, levels);
        SaveArrayAsPref(losingStreakKey, losingStreaks);

        //SaveLevels();
    }

    // adds to the streak of losses to the current operation
    public void AddLosingStreak()
    {
        // adds streak
        losingStreaks[randomOperation]++;
        // resets the winning streak
        winningStreaks[randomOperation] = 0;

        // checks if losing streaks meets demotion requirements
        if (losingStreaks[randomOperation] >= losingStreaksToDemote)
        {
            losingStreaks[randomOperation] = 0;
            if (levels[randomOperation] > 0)
            {
                // demote levels
                levels[randomOperation]--;
                Debug.Log(levels[randomOperation]);

            }
        }

        SaveArrayAsPref(levelsPrefsKey, levels);
        SaveArrayAsPref(streakKey, winningStreaks);
        SaveArrayAsPref(losingStreakKey, losingStreaks);
    }

   
    public void GenerateProblem()
    {
        numberList.Clear();
        // generate a problem with random numbers

        randomOperation = Random.Range(0, 4);
        
        // Makes sure that only the chosen operations are selected randomly
        while(!operations[randomOperation])
        {
            randomOperation = Random.Range(0, 4);
        }

        // the range in randomness will differ depending on difficulty.
        float num1 = 0;
        float num2 = 0;

        //operation = 3;
        if (randomOperation == 0) // Addition
        {
            if (levels[0] > additionRangeOfNums.Length - 1)
            {
                levels[0] = additionRangeOfNums.Length - 1;
            }

            //int tempLevel = level >= addingRangeOfNums.Length ? addingRangeOfNums.Length - 1 : level;
            num1 = (int)Random.Range(minAdditionNums[levels[0]], additionRangeOfNums[levels[0]] + 1);
            num2 = (int)Random.Range(minAdditionNums[levels[0]], additionRangeOfNums[levels[0]] + 1);

            operationString = "+";
            solution = num1 + num2;
        }
        else if (randomOperation == 1) // Subtraction
        {

            if (levels[1] > subtractionRangeOfNums.Length - 1)
            {
                levels[1] = subtractionRangeOfNums.Length - 1;
            }

            Debug.Log("Give negative solutions: " + giveNegativeSolutions);
            //int tempLevel = level - subtractingAppearanceLevel >= subtractingRangeOfNums.Length ? subtractingRangeOfNums.Length - 1 : level - subtractingAppearanceLevel;
            // make this number have a minimum of 1 if we don't want negatives.
            num1 = Random.Range(!giveNegativeSolutions ? 1 : minSubtractionNums[levels[1]], subtractionRangeOfNums[levels[1]] + 1);

            // if we're not giving negative nums, this number has to be less than or equal to num1
            num2 = (int)Random.Range(minSubtractionNums[levels[1]], !giveNegativeSolutions ? num1 : subtractionRangeOfNums[levels[1]] + 1);

            operationString = "-";
            solution = num1 - num2;
        }
        else if (randomOperation == 2) // Multiplication
        {
            if (levels[2] > multiplicationRangeOfNums.Length - 1)
            {
                levels[2] = multiplicationRangeOfNums.Length - 1;
            }
            //int tempLevel = level - multiplyingAppearanceLevel >= multiplyingRangeOfNums.Length ? multiplyingRangeOfNums.Length - 1 : level - multiplyingAppearanceLevel;
            num1 = (int)Random.Range(minMultiplicationNums[levels[2]], multiplicationRangeOfNums[levels[2]] + 1);
            num2 = (int)Random.Range(minMultiplicationNums[levels[2]], multiplicationRangeOfNums[levels[2]] + 1);

            operationString = "×";
            solution = num1 * num2;
        }
        else if (randomOperation == 3) // Division
        {

            if (levels[3] > divisionRangeOfNums.Length - 1)
            {
                levels[3] = divisionRangeOfNums.Length - 1;
            }
            //int tempLevel = level - dividingAppearanceLevel >= dividingRangeOfNums.Length ? dividingRangeOfNums.Length - 1: level - dividingAppearanceLevel;

            operationString = "÷";
            num1 = (int)Random.Range(minDivisionNums[levels[3]], divisionRangeOfNums[levels[3]] + 1);
            num2 = (int)Random.Range(minDivisionNums[levels[3]] + 1, Mathf.Ceil(divisionRangeOfNums[levels[3]] / 2 + 1));

            // make num1 a multiple of num2 so it can be cleanly divided
            num1 *= num2;

            solution = num1 / num2;
        }
        numberList.Add(num1);
        numberList.Add(num2);

        //Debug.Log(num1 + " " + operationString + " " + num2 + " = " + solution);
        string equation = num1 + " " + operationString + " " + num2 + " = ?";



        problemText.text = equation;
        problemText.gameObject.SetActive(true);

        canvasAnim.SetTrigger("FadeInText");

        // Shuffle the text For each spot in the array, pick a random item to swap into that spot.
        System.Random rand = new System.Random();
        for (int i = 0; i < possibleSolutionsText.Length - 1; i++)
        {
            int j = rand.Next(i, possibleSolutionsText.Length);
            TextMeshProUGUI temp = possibleSolutionsText[i];
            possibleSolutionsText[i] = possibleSolutionsText[j];
            possibleSolutionsText[j] = temp;
        }

        List<int> tempSolutions = new List<int>();
        // make possible solutions
        for (int i = 0; i < possibleSolutionsText.Length; i++)
        {
            int possibleSolution = (int)solution;

            // random addition
            if (i > 0)
            {
                // do this until the wrong solution is not equal to the actual solution
                while(possibleSolution == solution)
                {                
                    int number = Random.Range(-5, 5);
                    possibleSolution = (int)Mathf.Abs(solution + number);
                }            
            } else
            {
                solutionTransform = possibleSolutionsText[i].transform.parent.parent;
            }
            tempSolutions.Add(possibleSolution);
            possibleSolutionsText[i].text = possibleSolution.ToString();
            // TODO: make sure there are no duplicate possible solutions
        }

    }
}
