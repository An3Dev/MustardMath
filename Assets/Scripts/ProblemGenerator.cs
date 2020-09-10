using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemGenerator : MonoBehaviour
{

    //public enum Operations { Addition, Subtraction, Division, Multiplication };
    List<float> numberList = new List<float>();

    float solution;
    string operationString;

    private void Start()
    {
        // generate a problem with random numbers

        // add a random number to numberList

        float num1 = Random.Range(0, 10);
        float num2 = Random.Range(0, 10);

        int operation = Random.Range(0, 4);

        numberList.Add(num1);
        numberList.Add(num2);

        if (operation == 0)
        {
            operationString = "+";
            solution = num1 + num2;
            Debug.Log(num1 + " + " + num2 + " = " + solution);
        }

        foreach(float num in numberList)
        {
            Debug.Log(num.ToString());
        }
    }
}
