using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{

    public GameObject barrierPrefab;

    List<GameObject> barriers = new List<GameObject>();
    Transform player;

    Vector2 barrierSize;

    public int numOfRecycles = 0;
    private void Start()
    {
        barrierSize = barrierPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;

        barriers.Add(Instantiate(barrierPrefab, transform));
        barriers.Add(Instantiate(barrierPrefab, new Vector3(0, (numOfRecycles + 1) * barrierSize.y), Quaternion.identity, transform));
        Debug.Log(barrierSize.y);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void RecycleBarriers()
    {
        barriers[0].transform.position = new Vector3(0, (numOfRecycles + 2) * barrierSize.y);

        barriers[0].transform.SetAsLastSibling();
        GameObject temp = barriers[0];
        barriers.RemoveAt(0);
        barriers.Add(temp);

        numOfRecycles++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RecycleBarriers();
        }
    }
}
