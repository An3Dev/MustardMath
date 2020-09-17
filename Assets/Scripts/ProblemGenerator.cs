using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
public class ProblemGenerator : MonoBehaviour
{
    public TextMeshProUGUI problemText;
    public int additionLevel = 0, subtractionLevel = -1, multiplicationLevel = -1, divisionLevel = -1; // 0 is the easiest level.

    public TextMeshProUGUI[] possibleSolutionsText;

    List<float> numberList = new List<float>();

    float solution;
    string operationString;
    bool giveNegativeSolutions = false;

    // the difficulty at which negative numbers appear
    int negativeNumLevel = 4;

    public bool autoProgression = true;

    public bool addingOnly, subtractingOnly, multiplyingOnly, dividingOnly;

    // level at which subtracting problems appear
    public int subtractingAppearanceLevel = 3;
    public int multiplyingAppearanceLevel = 8;
    public int dividingAppearanceLevel = 15;

    // the 4 arrays below hold the max values that the student will be given depending on their level.
    int[] additionRangeOfNums = new int[] { 3, 5, 10, 20, 30, 40, 50, 60};

    int[] subtractionRangeOfNums = new int[] { 5, 7, 10, 20, 30, 40, 50, 60 };

    int[] multiplicationRangeOfNums = new int[] { 3, 5, 7, 10, 12 };

    int[] divisionRangeOfNums = new int[] { 3, 5, 7, 10, 12 };

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateProblem();
        }
    }

    void GenerateProblem()
    {

        numberList.Clear();
        Debug.Log("----------------------------");
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

        Debug.Log(maxOperation);
        int operation = Random.Range(0, maxOperation);

        // if the user chose a certain operation, override the random operation.
        if (addingOnly)
        {
            operation = 0;
        }
        else if (subtractingOnly)
        {
            operation = 1;
        }
        else if (multiplyingOnly)
        {
            operation = 2;
        }
        else if (dividingOnly)
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
            num1 = (int)Random.Range(0, additionRangeOfNums[additionLevel] + 1);
            num2 = (int)Random.Range(0, additionRangeOfNums[additionLevel] + 1);

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
            num1 = Random.Range(!giveNegativeSolutions ? 1 : 0, subtractionRangeOfNums[subtractionLevel] + 1);

            // if we're not giving negative nums, this number has to be less than or equal to num1
            num2 = (int)Random.Range(0, !giveNegativeSolutions ? num1 : subtractionRangeOfNums[subtractionLevel] + 1);

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
            num1 = (int)Random.Range(0, multiplicationRangeOfNums[multiplicationLevel] + 1);
            num2 = (int)Random.Range(0, multiplicationRangeOfNums[multiplicationLevel] + 1);

            operationString = "x";
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
            num1 = (int)Random.Range(0, divisionRangeOfNums[divisionLevel] + 1);
            num2 = (int)Random.Range(1, Mathf.Ceil(divisionRangeOfNums[divisionLevel] / 2 + 1));

            // make num1 a multiple of num2 so it can be cleanly divided
            num1 *= num2;

            solution = num1 / num2;
        }

        //Debug.Log(num1 + " " + operationString + " " + num2 + " = " + solution);
        string equation = num1 + " " + operationString + " " + num2 + " = ?";
        problemText.text = equation;

        //possibleSolutionsText[possibleSolutionsText.Length - 1].text = solution.ToString();

        // Shuffle the text
        // For each spot in the array, pick
        // a random item to swap into that spot.
        System.Random rand = new System.Random();
        for (int i = 0; i < possibleSolutionsText.Length - 1; i++)
        {
            int j = rand.Next(i, possibleSolutionsText.Length);
            TextMeshProUGUI temp = possibleSolutionsText[i];
            possibleSolutionsText[i] = possibleSolutionsText[j];
            possibleSolutionsText[j] = temp;
        }

        // make possible solutions
        for (int i = 0; i < possibleSolutionsText.Length; i++)
        {
            int possibleSolution = (int)solution;

            // random addition
            if (i > 0)
            {
                int number = Random.Range(-5, 5);
                possibleSolution = (int)Mathf.Abs(solution + number);
            }
            possibleSolutionsText[i].text = possibleSolution.ToString();
        }
    }
}
