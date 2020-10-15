using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public ProblemGenerator problemGen;

    public void UpdateText()
    {
        problemGen.UpdateText();
    }
}
