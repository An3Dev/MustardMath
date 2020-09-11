using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
public class ProblemGenerator : MonoBehaviour
{
    public TextMeshProUGUI problemText;
    public int level = 0; // 0 is the easiest level.



    List<float> numberList = new List<float>();

    float solution;
    string operationString;
    bool giveNegativeSolutions = false;

    // the difficulty at which negative numbers appear
    int negativeNumLevel = 4;

    // level at which subtracting problems appear
    public int subtractingAppearanceLevel = 3;
    public int multiplyingAppearanceLevel = 8;
    public int dividingAppearanceLevel = 15;

    // the 4 arrays below hold the max values that the student will be given depending on their level.
    int[] addingRangeOfNums = new int[] { 3, 5, 10, 20, 30, 40, 50, 60};

    int[] subtractingRangeOfNums = new int[] { 5, 7, 10, 20, 30, 40, 50, 60 };

    int[] multiplyingRangeOfNums = new int[] { 3, 5, 7, 10, 12 };

    int[] dividingRangeOfNums = new int[] { 3, 5, 7, 10, 12 };

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
        if (level < subtractingAppearanceLevel)
        {
            maxOperation = 1;
        }
        else if (level >= subtractingAppearanceLevel && level < multiplyingAppearanceLevel)
        {
            maxOperation = 2;
        } else if (level >= multiplyingAppearanceLevel && level < dividingAppearanceLevel)
        {
            maxOperation = 3;
        } else
        {
            maxOperation = 4;
        }

        int operation = Random.Range(0, maxOperation);

        // the range in randomness will differ depending on difficulty.

        float num1 = 0;
        float num2 = 0;

        if (operation == 0) // Addition
        {
            int tempLevel = level >= addingRangeOfNums.Length ? addingRangeOfNums.Length - 1 : level;
            num1 = (int) Random.Range(0, addingRangeOfNums[tempLevel] + 1);
            num2 = (int) Random.Range(0, addingRangeOfNums[tempLevel] + 1);

            operationString = "+";
            solution = num1 + num2;
        }
        else if (operation == 1) // Subtraction
        {
            int tempLevel = level - subtractingAppearanceLevel >= subtractingRangeOfNums.Length ? subtractingRangeOfNums.Length - 1 : level - subtractingAppearanceLevel;
            // make this number have a minimum of 1 if we don't want negatives.
            num1 = Random.Range(!giveNegativeSolutions ? 1 : 0, subtractingRangeOfNums[tempLevel] + 1);

            // if we're not giving negative nums, this number has to be less than or equal to num1
            num2 = (int) Random.Range(0, !giveNegativeSolutions ? num1 : subtractingRangeOfNums[tempLevel] + 1); 

            operationString = "-";
            solution = num1 - num2;
        }
        else if (operation == 2) // Multiplication
        {
            int tempLevel = level - multiplyingAppearanceLevel >= multiplyingRangeOfNums.Length ? multiplyingRangeOfNums.Length - 1 : level - multiplyingAppearanceLevel;
            num1 = (int)Random.Range(0, multiplyingRangeOfNums[tempLevel] + 1);
            num2 = (int)Random.Range(0, multiplyingRangeOfNums[tempLevel] + 1);

            operationString = "x";
            solution = num1 * num2;
        }
        else if (operation == 3) // Division
        {

            int tempLevel = level - dividingAppearanceLevel >= dividingRangeOfNums.Length ? dividingRangeOfNums.Length - 1: level - dividingAppearanceLevel;

            operationString = "÷";
            num1 = (int)Random.Range(0, dividingRangeOfNums[tempLevel] + 1);
            num2 = (int) Random.Range(1, Mathf.Ceil(dividingRangeOfNums[tempLevel] / 2 + 1));

            // make num1 a multiple of num2 so it can be cleanly divided
            num1 *= num2;

            solution = num1 / num2;
        }

        //Debug.Log(num1 + " " + operationString + " " + num2 + " = " + solution);
        string equation = num1 + " " + operationString + " " + num2 + " = ?";
        problemText.text = equation;

        Debug.Log(solution);

        for (int i = 0; i < 3; i++)
        {
            Debug.Log(solution + i);
        }

        //numberList.Add(num1);
        //numberList.Add(num2);
        //foreach (float num in numberList)
        //{
        //    Debug.Log(num.ToString());
        //}
    }
}
