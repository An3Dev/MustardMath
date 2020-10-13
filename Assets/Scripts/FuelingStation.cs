using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelingStation : MonoBehaviour
{

    public ProblemGenerator problemGenerator;
    public PlayerMovement playerMovement;

    public void AnimationEnded()
    {
        playerMovement.MoveFuelStation();
    }
}
