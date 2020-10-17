using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    public Animator anim;
    public Sprite[] collectableImages;
    public Sprite[] obstacleImages;
    public Collider2D collectableCollider;
    public Collider2D obstacleCollider;

    Rigidbody2D rb;
    private bool collectable = false;
    SpriteRenderer thisRenderer;

    public Vector2 collectableSize = new Vector2(1.5f, 2.2f);
    public Vector2 obstacleSize = new Vector2(2.5f, 3f);

    GameObject player;
    private void Awake()
    {
        thisRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    void ResetObject()
    {
        anim.SetTrigger("Reset");
        rb.angularVelocity = 0;
        rb.velocity = Vector3.zero;
    }

    // changes into a unique collectable
    public void TurnIntoCollectable()
    {
        collectable = true;
        ResetObject();

        int randIndex = Random.Range(0, collectableImages.Length);     
        thisRenderer.sprite = collectableImages[randIndex];
        float scale = Random.Range(collectableSize.x, collectableSize.y);
        transform.localScale = new Vector3(scale, scale, 1);

        obstacleCollider.enabled = false;
        collectableCollider.enabled = true;
        float randomTorque = Random.Range(-5, 5);
        rb.AddTorque(randomTorque);
    }

    // changes this object into an obstacle
    public void TurnIntoObstacle()
    {
        collectable = false;
        ResetObject();

        int randIndex = Random.Range(0, obstacleImages.Length);
        thisRenderer.sprite = obstacleImages[randIndex];

        float scale = Random.Range(obstacleSize.x, obstacleSize.y);
        transform.localScale = new Vector3(scale, scale, 1);

        obstacleCollider.enabled = true;
        collectableCollider.enabled = false;
        float randomTorque = Random.Range(-3, 3);
        rb.AddTorque(randomTorque);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collect trash
        if (collision.gameObject.CompareTag("Player"))
        {
            // play collected animation
            anim.SetTrigger("Collected");
            //player.SendMessage("CollectedTrash");
        }
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void Collected()
    {
        player.SendMessage("CollectedTrash");
    }
}
