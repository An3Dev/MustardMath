using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocity = 5;
    public float changeDirectionBoost = 2;
    public float maxSpeed = 5;
    public Transform cam;
    public float turnSpeed = 90;
    public Transform exhaust;
    public Transform particleSystemTransform;

    Vector3 cameraOffset;

    Rigidbody2D rb;

    float targetDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = cam.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            targetDir = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetDir = 1;
        } else
        {
            targetDir = 0;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, rb.transform.position + cameraOffset, 0.9f);

    }
    private void FixedUpdate()
    {
        float currentDir = rb.velocity.normalized.x;

        rb.MoveRotation(transform.rotation.eulerAngles.z + turnSpeed * Time.deltaTime * -targetDir);

        rb.velocity = transform.up * velocity * Time.fixedDeltaTime;
    }
}
