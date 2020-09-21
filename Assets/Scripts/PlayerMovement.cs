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
    public Transform fuelTransform;
    public ProblemGenerator problemGenerator;
    public float slowMotionProximity = 5;
    public GameObject fuelingStation;

    public float fuelPerSecond = 2;
    Vector3 cameraOffset;

    Rigidbody2D rb;

    float targetDir;

    float fuelUsed = 0;
    float maxFuel = 100;
    bool cancelSlowMo = false;
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

        // if space is pressed, cancel slow motion.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            cancelSlowMo = true;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, rb.transform.position + cameraOffset, 0.9f);

        fuelTransform.localPosition = new Vector3(0, 0 - (fuelUsed / maxFuel) * 0.9f, -0.1f);
        //if (fuelTransform.localPosition.y < -0.9f)
        //{
        //    fuelTransform.localPosition = new Vector3(0, 0, -0.1f);
        //}
        if (fuelUsed >= maxFuel)
        {
            Debug.Log("No fuel");
            fuelUsed = 0;
        }

        // if rocket is close enough to the fueling station, slow down time to give use time to think.
        if (!cancelSlowMo && (transform.position - fuelingStation.transform.position).sqrMagnitude < slowMotionProximity * slowMotionProximity)
        {
            Time.timeScale = 0.4f;
        }

        fuelUsed += fuelPerSecond * Time.deltaTime;
    }
    private void FixedUpdate()
    {
        rb.MoveRotation(transform.rotation.eulerAngles.z + turnSpeed * Time.deltaTime * -targetDir);

        rb.velocity = transform.up * velocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // detect if hit a fueling tank
        if (collision.gameObject.layer == LayerMask.NameToLayer("FuelingArea"))
        {
            // Generate problem
            problemGenerator.GenerateProblem();
            fuelingStation.GetComponent<Collider2D>().enabled = false;
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("FuelingStation"))
        {
            // Check is player got answer right
            if (problemGenerator.solutionTransform == collision.transform)
            {
                fuelUsed = 0;
                Debug.Log("Correct, give gas");
                
                // move fueling station to next location
                fuelingStation.transform.position += Vector3.up * 40;
                fuelingStation.GetComponent<Collider2D>().enabled = true;

                // make time back to normal
                Time.timeScale = 1;
                cancelSlowMo = false;

                for (int i = 0; i < fuelingStation.transform.childCount; i++)
                {
                    fuelingStation.transform.GetChild(i).gameObject.SetActive(true);
                }
                // add satisfying effect and dissapear the fueling station.
                // once it's not visible, move it to the next location.

                // show correct equation for a few seconds, then dissolve it.
                problemGenerator.CorrectAnswer();
            } else
            {
                Debug.Log("Incorrect");
                collision.transform.gameObject.SetActive(false);
                // if incorrect, the chosen fuel container will dissapear and the player can choose again.
                // The player will get less fuel for getting an answer right the second try.
            }

        }
    }
}
