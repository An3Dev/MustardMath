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

    public SpriteRenderer fuelingTankRenderer1, fuelingTankRenderer2, fuelingTankRenderer3, fuelingTankRenderer4;
    public TextMeshProUGUI tankTextMesh1, tankTextMesh2, tankTextMesh3, tankTextMesh4;

    public float wallForce = 2500;

    public float distanceBetweenAnswers = 50;

    public float fuelPerSecond = 2;

    public float correctFuelDissolveTime = 1f;
    public float incorrectFuelDissolveTime = 0.5f;

    public ParticleSystem correctAnswerPS, incorrectAnswerPS;

    Vector3 cameraOffset;



    Rigidbody2D rb;

    float targetDir;

    float fuelUsed = 0;
    float maxFuel = 100;
    bool cancelSlowMo = false;

    float fuelOffCameraOffset = -10;

    int tries = 0;

    bool gameOver = false;

    float lastMoveTime = 0;
    
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

        if (transform.position.y > fuelingStation.transform.position.y - fuelOffCameraOffset)
        {
            // Spawn the fueling station on top of the player
            MoveFuelStation();
        }

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

        fuelTransform.localPosition = new Vector3(0, 0.2f - (fuelUsed / maxFuel) * 0.9f, -0.1f);


        if (fuelUsed >= maxFuel)
        {
            Debug.Log("No fuel");
            gameOver = true;
            SceneManager.LoadScene(0);
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
                } else if (tries == 1)
                {
                    fuelUsed -= maxFuel / 3;
                    // don't spawn powerup
                } else if (tries == 3)
                {
                    fuelUsed -= 1 / 4;
                    // don't spawn powerup
                }

                if(fuelUsed < 0)
                {
                    fuelUsed = 0;
                }

                tries = 0;

                Debug.Log("Correct, give gas");

                //MoveFuelStation();

                // spawn power up 

                // show particle system.
                correctAnswerPS.transform.position = collision.transform.position;
                correctAnswerPS.Play();

                // fade away images of all fuel tanks.
                for(int i = 0; i < 4; i++)
                {
                    StartCoroutine(FadeImage(true, fuelingStation.transform.GetChild(i).GetComponent<SpriteRenderer>(), fuelingStation.transform.GetChild(i) == collision.transform, true));
                    StartCoroutine(FadeImage(true, fuelingStation.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(), fuelingStation.transform.GetChild(i) == collision.transform, true));
                }

                // show correct equation for a few seconds, then dissolve it.


                problemGenerator.CorrectAnswer();
            } else
            {
                Debug.Log("Incorrect");

                incorrectAnswerPS.transform.position = collision.transform.position;
                incorrectAnswerPS.Play();

                StartCoroutine(FadeImage(true, collision.transform.GetComponent<SpriteRenderer>(), false, false));
                StartCoroutine(FadeImage(true, collision.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(), false, false));


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

    // moves the rocket away from wall
    void MoveRocket(Vector3 collisionPoint)
    {
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
        rb.AddTorque(-xDir * 360);
        Debug.Log(xDir);
    }

    IEnumerator FadeImage(bool fadeAway, SpriteRenderer renderer, bool isCorrectAnswer, bool moveFuel)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            float fadeTime = isCorrectAnswer ? correctFuelDissolveTime : incorrectFuelDissolveTime;
            if (moveFuel)
            {
                renderer.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                renderer.color = new Color(renderer.color.r, renderer.color.g, 1);
            }

            // loop over 1 second backwards
            for (float i = fadeTime; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, i);
                if (i <= 0.01f)
                {
                    if (moveFuel)
                    {
                        Debug.Log("Faded away");

                        MoveFuelStation();
                    }

                }
                yield return null;
            }
        }
        // fade from transparent to opaque
        //else
        //{
        //    // loop over 1 second
        //    for (float i = 0; i <= 1; i += Time.deltaTime)
        //    {
        //        // set color with i as alpha
        //        renderer.color = new Color(1, 1, 1, i);
        //        yield return null;
        //    }
        //}
    }

    IEnumerator FadeImage(bool fadeAway, TextMeshProUGUI text, bool isCorrectAnswer, bool moveFuel)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            float fadeTime = isCorrectAnswer ? correctFuelDissolveTime : incorrectFuelDissolveTime;
            // loop over 1 second backwards
            for (float i = fadeTime; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                text.color = new Color(text.color.r, text.color.g, text.color.b, i);
                if (i <= 0.01f)
                {
                    // if image faded away, move the fuel
                    if (!isCorrectAnswer && moveFuel)
                    {
                        MoveFuelStation();
                    }
                }
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }

    void MoveFuelStation()
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

        // enable all of the fueling tanks.

        fuelingTankRenderer1.color = new Color(fuelingTankRenderer1.color.r, fuelingTankRenderer1.color.g, fuelingTankRenderer1.color.b, 1);
        fuelingTankRenderer2.color = new Color(fuelingTankRenderer2.color.r, fuelingTankRenderer2.color.g, fuelingTankRenderer2.color.b, 1);
        fuelingTankRenderer3.color = new Color(fuelingTankRenderer3.color.r, fuelingTankRenderer3.color.g, fuelingTankRenderer3.color.b, 1);
        fuelingTankRenderer4.color = new Color(fuelingTankRenderer4.color.r, fuelingTankRenderer4.color.g, fuelingTankRenderer4.color.b, 1);

        tankTextMesh1.color = new Color(255, 255, 255, 1);
        tankTextMesh2.color = new Color(255, 255, 255, 1);
        tankTextMesh3.color = new Color(255, 255, 255, 1);
        tankTextMesh4.color = new Color(255, 255, 255, 1);

        Debug.Log("Test");

        for (int i = 0; i < fuelingStation.transform.childCount; i++)
        {
            fuelingStation.transform.GetChild(i).gameObject.SetActive(true);

            //SpriteRenderer renderer = fuelingStation.transform.GetChild(i).GetComponent<SpriteRenderer>();
            //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);

            //fuelingStation.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            fuelingStation.transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>().enabled = true;

        }
    }
}
