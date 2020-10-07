using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

public class ProblemGenerator : MonoBehaviour
{
    public TextMeshProUGUI problemText;
    public static int additionLevel = 0, subtractionLevel = 0, multiplicationLevel = 0, divisionLevel = 0; // 0 is the easiest level.

    public TextMeshProUGUI[] levelText;

    public List<int> levels;
    public TextMeshProUGUI[] possibleSolutionsText;

    List<float> numberList = new List<float>();

    float solution;
    string operationString;
    bool giveNegativeSolutions = false;

    // the difficulty at which negative numbers appear
    int negativeNumLevel = 4;

    public bool autoProgression = true;

    public static bool additionOnly, subtractionOnly, multiplicationOnly, divisionOnly;

    // level at which subtracting problems appear
    public int subtractingAppearanceLevel = 3;
    public int multiplyingAppearanceLevel = 8;
    public int dividingAppearanceLevel = 15;

    string levelsPrefsKey;

    // the 4 arrays below hold the max values that the student will be given depending on their level.
    int[] additionRangeOfNums = new int[] { 3, 5, 10, 20, 30, 40, 50, 60 };
    int[] minAdditionNums = new int[] { 0, 1, 3, 10, 10, 10, 10, 10 }; 

    int[] subtractionRangeOfNums = new int[] { 5, 7, 10, 20, 30, 40, 50, 60 };
    int[] minSubtractionNums = new int[] { 0, 1, 2, 3, 5, 10, 10, 10};

    int[] multiplicationRangeOfNums = new int[] { 3, 5, 7, 10, 12 };
    int[] minMultiplicationNums = new int[] { 0, 1, 2, 3, 5, 7 };

    int[] divisionRangeOfNums = new int[] { 3, 5, 7, 10, 12 };
    int[] minDivisionNums = new int[] { 0, 1, 2, 3, 5, 5};
    public Transform solutionTransform;

    private void Start()
    {
        SetLevels();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            levelText[0].text = "Level " + (additionLevel + 1);
            levelText[1].text = "Level " + (subtractionLevel + 1);
            levelText[2].text = "Level " + (multiplicationLevel + 1);
            levelText[3].text = "Level " + (divisionLevel + 1);
        }

        
    }

    void SetLevels()
    {
        // Get data about levels
        string levelsData = PlayerPrefs.GetString(levelsPrefsKey, "0,0,0,0");

        String[] array = levelsData.Split(',');

        for (int i = 0; i < array.Length; i++)
        {
            //levels.Add(int.Parse(array[i]));
            if (i == 0)
            {
                additionLevel = int.Parse(array[i]);
            }
            else if (i == 1)
            {
                subtractionLevel = int.Parse(array[i]);
            }
            else if (i == 2)
            {
                multiplicationLevel = int.Parse(array[i]);
            }
            else if (i == 3)
            {
                divisionLevel = int.Parse(array[i]);
            }
        }
    }

    public void StartGame(int operation)
    {
        if (operation == 0)
        {
            additionOnly = true; // adding only
            subtractionOnly = true;
            multiplicationOnly = false;
            divisionOnly = false;

        } else if (operation == 1)
        {
            additionOnly = false;
            subtractionOnly = true; // subtracting only          
            multiplicationOnly = false;
            divisionOnly = false;
        } else if (operation == 2)
        {
            additionOnly = false;
            subtractionOnly = false;     
            multiplicationOnly = true; // multiplication only
            divisionOnly = false;
        } else if (operation == 3)
        {
            additionOnly = false;
            subtractionOnly = false;        
            multiplicationOnly = false;
            divisionOnly = true; // division only
        } else
        {
            additionOnly = false;
            subtractionOnly = false;       
            multiplicationOnly = false;
            divisionOnly = false;
        }

        SceneManager.LoadScene(1);
    }

    public void CorrectAnswer()
    {
        problemText.text = numberList[0] + " " + operationString + " " + numberList[1] + " = " + solution;
        // make animation showing the correct answer. then make it dissapear.


    }

    void SaveLevels()
    {
        string levels = "";

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                levels += additionLevel.ToString();
            }
            else if (i == 1)
            {
                levels += "," + subtractionLevel.ToString();
            }
            else if (i == 2)
            {
                levels += "," + multiplicationLevel.ToString();
            }
            else if (i == 3)
            {
                levels += "," + divisionLevel.ToString();
            }

        }

        PlayerPrefs.SetString(levelsPrefsKey, levels);
    }

    public void GenerateProblem()
    {

        numberList.Clear();
        // generate a problem with random numbers

        // add a random number to numberList

        int maxOperation = 1;

        if (autoProgression)
        {
            if (divisionLevel > -1)
            {
                maxOperation = 4;
            }
            else if (multiplicationLevel > -1)
            {
                maxOperation = 3;
            }
            else if (subtractionLevel > -1)
            {
                maxOperation = 2;
            }
        }

        int operation = Random.Range(0, maxOperation);

        // if the user chose a certain operation, override the random operation.
        if (additionOnly)
        {
            operation = 0;
        }
        else if (subtractionOnly)
        {
            operation = 1;
        }
        else if (multiplicationOnly)
        {
            operation = 2;
        }
        else if (divisionOnly)
        {
            operation = 3;
        }

        // the range in randomness will differ depending on difficulty.
        float num1 = 0;
        float num2 = 0;

        //operation = 3;
        if (operation == 0) // Addition
        {
            if (additionLevel > additionRangeOfNums.Length - 1)
            {
                additionLevel = additionRangeOfNums.Length - 1;
            }

            //int tempLevel = level >= addingRangeOfNums.Length ? addingRangeOfNums.Length - 1 : level;
            num1 = (int)Random.Range(minAdditionNums[additionLevel], additionRangeOfNums[additionLevel] + 1);
            num2 = (int)Random.Range(minAdditionNums[additionLevel], additionRangeOfNums[additionLevel] + 1);

            operationString = "+";
            solution = num1 + num2;
        }
        else if (operation == 1) // Subtraction
        {

            if (subtractionLevel > subtractionRangeOfNums.Length - 1)
            {
                subtractionLevel = subtractionRangeOfNums.Length - 1;
            }
            //int tempLevel = level - subtractingAppearanceLevel >= subtractingRangeOfNums.Length ? subtractingRangeOfNums.Length - 1 : level - subtractingAppearanceLevel;
            // make this number have a minimum of 1 if we don't want negatives.
            num1 = Random.Range(!giveNegativeSolutions ? 1 : minSubtractionNums[subtractionLevel], subtractionRangeOfNums[subtractionLevel] + 1);

            // if we're not giving negative nums, this number has to be less than or equal to num1
            num2 = (int)Random.Range(minSubtractionNums[subtractionLevel], !giveNegativeSolutions ? num1 : subtractionRangeOfNums[subtractionLevel] + 1);

            operationString = "-";
            solution = num1 - num2;
        }
        else if (operation == 2) // Multiplication
        {
            if (multiplicationLevel > multiplicationRangeOfNums.Length - 1)
            {
                multiplicationLevel = multiplicationRangeOfNums.Length - 1;
            }
            //int tempLevel = level - multiplyingAppearanceLevel >= multiplyingRangeOfNums.Length ? multiplyingRangeOfNums.Length - 1 : level - multiplyingAppearanceLevel;
            num1 = (int)Random.Range(minMultiplicationNums[multiplicationLevel], multiplicationRangeOfNums[multiplicationLevel] + 1);
            num2 = (int)Random.Range(minMultiplicationNums[multiplicationLevel], multiplicationRangeOfNums[multiplicationLevel] + 1);

            operationString = "×";
            solution = num1 * num2;
        }
        else if (operation == 3) // Division
        {

            if (divisionLevel > divisionRangeOfNums.Length - 1)
            {
                divisionLevel = divisionRangeOfNums.Length - 1;
            }
            //int tempLevel = level - dividingAppearanceLevel >= dividingRangeOfNums.Length ? dividingRangeOfNums.Length - 1: level - dividingAppearanceLevel;

            operationString = "÷";
            num1 = (int)Random.Range(minDivisionNums[divisionLevel], divisionRangeOfNums[divisionLevel] + 1);
            num2 = (int)Random.Range(minDivisionNums[divisionLevel] + 1, Mathf.Ceil(divisionRangeOfNums[divisionLevel] / 2 + 1));

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
