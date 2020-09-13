using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocity = 5;
    public float changeDirectionBoost = 2;
    public float maxSpeed = 5;
    public Transform cam;


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
        cam.transform.position = Vector3.Lerp(cam.transform.position, rb.transform.position + cameraOffset, 1f);

    }

    private void LateUpdate()
    {

    }
    private void FixedUpdate()
    {
        float currentDir = rb.velocity.normalized.x;

        Vector3 force = Vector3.zero;

        force = Vector3.right * targetDir * velocity;
        //if (targetDir == -1)
        //{
        //    force += 
        //    rb.AddForce(Vector3.right * targetDir * velocity);

        //}
        //else if (targetDir == 1)
        //{
        //    force += Vector3.right * targetDir * velocity;

        //    rb.AddForce(Vector3.right * targetDir * velocity);
        //}

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -30 * targetDir));
        //transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, 0, -30 * targetDir), Mathf.Abs(transform.rotation.eulerAngles.z) / 30));

        if (currentDir != targetDir)
        {
            force += Vector3.right * targetDir * velocity * changeDirectionBoost;
            //rb.AddTorque(targetDir * -5 * changeDirectionBoost);

        }

        rb.AddForce(force * Time.fixedDeltaTime, ForceMode2D.Impulse);

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.AddForce(-rb.velocity);
        }

        //rb.AddTorque(targetDir * -5);


    }
}
