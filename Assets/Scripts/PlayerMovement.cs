using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
    public GameObject spawnedObjectPrefab;
    public float collectableProbability = 0.4f;
    public float obstacleProbability = 0.4f;
    public float numberOfObjects;
    public Transform floatingObjectParent;

    public float wallForce = 2500;

    public float distanceBetweenAnswers = 50;

    public float fuelPerSecond = 2;

    public AudioSource audioSource;

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

    float trashCollectionFuelReward = 5;

    public ParticleSystem exhaustOne, exhaustTwo;

    public AudioClip collectGasSoundEffect, wrongAnswerSoundEffect, correctAnswerSFX;
    public AudioSource soundEffectsAudioSource;

    public GameObject pauseMenu;
    
    public TextMeshProUGUI gameOverMessage;
    String[] gameOverMessages = new String[] { "Great Job!", "Keep Trying!", "You're Improving!", "Nice Job!", "You Did Awesome!" };

    float cancelSlowMoTime = 0.1f;
    float lastFingerHold;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = cam.position - transform.position;
        fuelTankColor = fuelingTankRenderer1.color;

        SpawnObstacles(fuelingStation.transform.position.y);
        Time.timeScale = 1;
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

        if (Input.touchCount > 1 && Time.time - lastFingerHold > cancelSlowMoTime) 
        {
            Time.timeScale = 1f;
            audioSource.pitch = 1;
            cancelSlowMo = true;
            lastFingerHold = Time.time;
            Debug.Log("Cancel");
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) && !turningOffWall)
        {
            targetDir = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) && !turningOffWall)
        {
            targetDir = 1;
        } else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                targetDir = -1;
            }
            if (Input.mousePosition.x > Screen.width / 2)
            {
                targetDir = 1;
            }
        }
        else
        {
            targetDir = 0;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                Pause();
            }else
            {

                Resume();
            }
        }

        // if space is pressed, cancel slow motion.
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Time.timeScale = 1;
            audioSource.pitch = 1;
            cancelSlowMo = true;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, rb.transform.position + cameraOffset, 0.9f);

        fuelTransform.localPosition = new Vector3(0, 0.216f - (fuelUsed / maxFuel) * 1.396f, -0.1f);


        if (fuelUsed >= maxFuel && !gameOver)
        {
            gameOver = true;
            audioSource.Stop();
            canvasAnimator.SetTrigger("GameOver");
            exhaust.gameObject.SetActive(false);
            exhaustOne.Stop();
            exhaustTwo.Stop();

            int randomIndex = UnityEngine.Random.Range(0, gameOverMessages.Length);

            gameOverMessage.text = gameOverMessages[randomIndex];
            
        }

        // if rocket is close enough to the fueling station, slow down time to give use time to think.
        if (!cancelSlowMo && (transform.position - fuelingStation.transform.position).sqrMagnitude < slowMotionProximity * slowMotionProximity)
        {
            Time.timeScale = 0.5f;
            audioSource.pitch = 0.5f;

        }

        fuelUsed += fuelPerSecond * Time.deltaTime;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        cancelSlowMo = true;
        Time.timeScale = 0.01f;
        audioSource.pitch = 0.01f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        cancelSlowMo = true;
        Time.timeScale = 1;
        audioSource.pitch = 1;
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


    void CollectedTrash()
    {
        fuelUsed -= trashCollectionFuelReward;
        soundEffectsAudioSource.PlayOneShot(collectGasSoundEffect);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // detect if is in area to show problem
        if (collision.gameObject.layer == LayerMask.NameToLayer("FuelingArea"))
        {
            // Generate problem
            problemGenerator.GenerateProblem();
            fuelingStation.GetComponent<Collider2D>().enabled = false;
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("FuelingStation"))
        {
            cancelSlowMo = true;
            Time.timeScale = 1;
            audioSource.pitch = 1;

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
                
                fuelTankAnimator1.SetTrigger(fuelTankAnimator1.transform == collision.transform ? "Fade" : "SimpleFade");
                fuelTankAnimator2.SetTrigger(fuelTankAnimator2.transform == collision.transform ? "Fade" : "SimpleFade");
                fuelTankAnimator3.SetTrigger(fuelTankAnimator3.transform == collision.transform ? "Fade" : "SimpleFade");
                fuelTankAnimator4.SetTrigger(fuelTankAnimator4.transform == collision.transform ? "Fade" : "SimpleFade");


                soundEffectsAudioSource.PlayOneShot(correctAnswerSFX);

                problemGenerator.PlayCorrectAnswerAnimation();

            } else
            {
                incorrectAnswerPS.transform.position = collision.transform.position;
                incorrectAnswerPS.Play();

                collision.transform.GetComponent<Animator>().SetTrigger("Wrong");
                problemGenerator.PlayWrongAnswerAnimation();
                soundEffectsAudioSource.PlayOneShot(wrongAnswerSoundEffect);


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

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            // add force in opposite direction
            MoveRocket(collision.GetContact(0).point);
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
        if (Time.timeSinceLevelLoad - lastMoveTime < 1f)
        {
            return;
        }

        lastMoveTime = Time.timeSinceLevelLoad;
        // move fueling station to next location
        fuelingStation.transform.position += Vector3.up * distanceBetweenAnswers;
        fuelingStation.GetComponent<Collider2D>().enabled = true;

        // make time back to normal
        Time.timeScale = 1;
        audioSource.pitch = 1;

        cancelSlowMo = false;

        fuelTankAnimator1.SetTrigger("Reset");
        fuelTankAnimator2.SetTrigger("Reset");

        fuelTankAnimator3.SetTrigger("Reset");

        fuelTankAnimator4.SetTrigger("Reset");

        SpawnObstacles(fuelingStation.transform.position.y);

        // enable all of the fueling tanks
        problemGenerator.FadeOutText();

        for (int i = 0; i < fuelingStation.transform.childCount; i++)
        {
            fuelingStation.transform.GetChild(i).gameObject.SetActive(true);

            fuelingStation.transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }


    #region object spawner

    Vector3 CalculatePosition(float fuelingStationPos, int i)
    {
        return new Vector3(Random.Range(-8.5f, 8.5f), fuelingStationPos - distanceBetweenAnswers + 5 + i * ((distanceBetweenAnswers - 10) / numberOfObjects), 3);
    }

    int GetRandomObject()
    {
        float prob = Random.Range(0, 100) / 100.0f;
        int spawnedObject = 2;
        if (prob <= collectableProbability)
        {
            spawnedObject = 0;
        }
        else if (prob > collectableProbability && prob <= (obstacleProbability + collectableProbability))
        {
            spawnedObject = 1;
        } else
        {
            spawnedObject = 2;
        }
        return spawnedObject;
    }

    void SpawnObstacles(float fuelingStationPos)
    {
        if (floatingObjectParent.childCount == 0)
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                int randomObjIndex = GetRandomObject();
                if (randomObjIndex == 2)
                {
                    GameObject notSpawned = Instantiate(spawnedObjectPrefab, CalculatePosition(fuelingStationPos, i), Quaternion.identity, floatingObjectParent);
                    notSpawned.SetActive(false);
                }

                GameObject obj = Instantiate(spawnedObjectPrefab, CalculatePosition(fuelingStationPos, i), Quaternion.identity, floatingObjectParent);
                SpawnedObject spawnedObject = obj.GetComponent<SpawnedObject>();
                if (randomObjIndex == 0)
                {
                    spawnedObject.TurnIntoCollectable();
                } else if (randomObjIndex == 1)
                {
                    spawnedObject.TurnIntoObstacle();
                }
            }

            fuelingStationPos += distanceBetweenAnswers;

            for (int i = 0; i < numberOfObjects; i++)
            {
                int randomObjIndex = GetRandomObject();
                if (randomObjIndex == 2)
                {
                    GameObject notSpawned = Instantiate(spawnedObjectPrefab, CalculatePosition(fuelingStationPos, i), Quaternion.identity, floatingObjectParent);
                    SpawnedObject test = notSpawned.GetComponent<SpawnedObject>();
                    test.TurnIntoObstacle();
                    notSpawned.SetActive(false);

                    continue;
                }

                GameObject obj = Instantiate(spawnedObjectPrefab, CalculatePosition(fuelingStationPos, i), Quaternion.identity, floatingObjectParent);
                SpawnedObject spawnedObject = obj.GetComponent<SpawnedObject>();
                if (randomObjIndex == 0)
                {
                    spawnedObject.TurnIntoCollectable();
                }
                else if (randomObjIndex == 1)
                {
                    spawnedObject.TurnIntoObstacle();
                }
            }
            return;
        }
        else
        {
            fuelingStationPos += distanceBetweenAnswers;
            for (int i = 0; i < numberOfObjects; i++)
            {
                int randomObjIndex = GetRandomObject();
                if (randomObjIndex == 2)
                {
                    // disable this object
                    continue;
                }

                Transform obj = floatingObjectParent.GetChild(0);
                obj.gameObject.SetActive(true);
                obj.position = CalculatePosition(fuelingStationPos, i);
                SpawnedObject spawnedObject = obj.GetComponent<SpawnedObject>();

                if (randomObjIndex == 0)
                {
                    spawnedObject.TurnIntoCollectable();
                }
                else if (randomObjIndex == 1)
                {
                    spawnedObject.TurnIntoObstacle();
                }

                floatingObjectParent.GetChild(0).SetAsLastSibling();
            } 
        }// Hi Mr. Ceara       
    }

    #endregion object spawner
}
