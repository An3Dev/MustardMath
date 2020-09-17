using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{

    public GameObject barrierPrefab;

    List<GameObject> barriers = new List<GameObject>();
    private void Start()
    {
        barriers.Add(Instantiate(barrierPrefab));
        barriers.Add(Instantiate(barrierPrefab));


    }

}
