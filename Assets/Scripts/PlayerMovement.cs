using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Animator canvasAnimator;

    public SpriteRenderer fuelingTankRenderer1, fuelingTankRenderer2, fuelingTankRenderer3, fuelingTankRenderer4;
    //public TextMeshProUGUI tankTextMesh1, tankTextMesh2, tankTextMesh3, tankTextMesh4;
    public Animator fuelTankAnimator1, fuelTankAnimator2, fuelTankAnimator3, fuelTankAnimator4;

    public float wallForce = 2500;

    public float distanceBetweenAnswers = 50;

    public float fuelPerSecond = 2;

    //public float correctFuelDissolveTime = 1f;
    //public float incorrectFuelDissolveTime = 0.5f;

    public ParticleSystem correctAnswerPS, incorrectAnswerPS;

    Vector3 cameraOffset;

    private Color fuelTankColor;


    Rigidbody2D rb;

    float targetDir;

    float fuelUsed = 0;
    public float maxFuel = 100;
    bool cancelSlowMo = false;

    float fuelOffCameraOffset = -10;

    int tries = 0;

    bool gameOver = false;

    float lastMoveTime = 0;

    bool turningOffWall = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = cam.position - transform.position;
        fuelTankColor = fuelingTankRenderer1.color;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Menu))
        {
            SceneManager.LoadScene(0);
        }

        // if player passed the fueling station
        if (transform.position.y > fuelingStation.transform.position.y - fuelOffCameraOffset)
        {
            // Spawn the fueling station on top of the player
            MoveFuelStation();
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) && !turningOffWall)
        {
            targetDir = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) && !turningOffWall)
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

        fuelTransform.localPosition = new Vector3(0, 0.216f - (fuelUsed / maxFuel) * 1.396f, -0.1f);


        if (fuelUsed >= maxFuel && !gameOver)
        {
            gameOver = true;
            canvasAnimator.SetTrigger("GameOver");
            exhaust.gameObject.SetActive(false);
            //SceneManager.LoadScene(0);
            // show stats
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

        if (gameOver) 
        {
            return;
        }

        rb.velocity = transform.up * velocity * Time.fixedDeltaTime;
    }

    public void OpenMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void TryAgain()
    {
        SceneManager.LoadScene(1);
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
            // If player got answer right
            if (problemGenerator.solutionTransform == collision.transform)
            {
                if (tries == 0)
                {
                    fuelUsed -= maxFuel / 2;
                    problemGenerator.AddToStreak();

                }
                else if (tries == 1)
                {
                    fuelUsed -= maxFuel / 3;
                    problemGenerator.AddLosingStreak();

                }
                else if (tries == 2)
                {
                    fuelUsed -= maxFuel / 4;
                    // reset streak.
                    problemGenerator.AddLosingStreak();
                }
                else
                {
                    problemGenerator.AddLosingStreak();

                }

                if (fuelUsed < 0)
                {
                    fuelUsed = 0;
                }

                tries = 0;

                // show particle system.
                correctAnswerPS.transform.position = collision.transform.position;
                correctAnswerPS.Play();

                // fade away images of all fuel tanks.
                fuelTankAnimator1.SetTrigger("Fade");
                fuelTankAnimator2.SetTrigger("Fade");
                fuelTankAnimator3.SetTrigger("Fade");
                fuelTankAnimator4.SetTrigger("Fade");


                // show correct equation for a few seconds, then dissolve it.


                // obstacles. fuel position.y - 20

                problemGenerator.UpdateText();

            } else
            {
                incorrectAnswerPS.transform.position = collision.transform.position;
                incorrectAnswerPS.Play();

                collision.transform.GetComponent<Animator>().SetTrigger("Wrong");

                // disable collider
                collision.transform.GetComponent<Collider2D>().enabled = false;

                //collision.transform.gameObject.SetActive(false);

                tries++;

                // if incorrect, the chosen fuel container will dissapear and the player can choose again.
                // The player will get less fuel for getting an answer right the second try.
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            // add force in opposite direction
            MoveRocket(collision.GetContact(0).point);

        } else if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            // add force in opposite direction
            MoveRocket(collision.GetContact(0).point);

        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            // add force in opposite direction
            turningOffWall = false;

        }
    }
    // moves the rocket away from wall
    void MoveRocket(Vector3 collisionPoint)
    {
        turningOffWall = true;
        int xDir = 0;
        if (collisionPoint.x < transform.position.x)
        {
            // force right
            xDir = 1;

        } else
        {
            // force to the left
            xDir = -1;
        }

        Vector2 dir = new Vector2(xDir, 0.5f);
        rb.AddForce(dir * wallForce, ForceMode2D.Impulse);
        rb.AddTorque(-xDir * 90);
    }

    public void MoveFuelStation()
    {
        if (Time.timeSinceLevelLoad - lastMoveTime < 0.5f)
        {
            return;
        }

        lastMoveTime = Time.timeSinceLevelLoad;
        // move fueling station to next location
        fuelingStation.transform.position += Vector3.up * distanceBetweenAnswers;
        fuelingStation.GetComponent<Collider2D>().enabled = true;

        // make time back to normal
        Time.timeScale = 1;
        cancelSlowMo = false;

        fuelTankAnimator1.SetTrigger("Reset");
        fuelTankAnimator2.SetTrigger("Reset");

        fuelTankAnimator3.SetTrigger("Reset");

        fuelTankAnimator4.SetTrigger("Reset");

        // enable all of the fueling tanks

        for (int i = 0; i < fuelingStation.transform.childCount; i++)
        {
            fuelingStation.transform.GetChild(i).gameObject.SetActive(true);

            fuelingStation.transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
