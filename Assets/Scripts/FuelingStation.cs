using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelingStation : MonoBehaviour
{

    public ProblemGenerator problemGenerator;
    public PlayerMovement playerMovement;
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        Debug.Log("Show problem");
    //        problemGenerator.GenerateProblem();

    //        // slow down time when player is close
    //    }
    //}

    public void AnimationEnded()
    {
        playerMovement.MoveFuelStation();
    }
}
