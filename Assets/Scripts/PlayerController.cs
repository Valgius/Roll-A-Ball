using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    [HideInInspector]
    public float baseSpeed;
    private Rigidbody rb;
    private int pickupCount;
    GameObject resetPoint;
    bool resetting = false;
    bool grounded = true;
    bool activePowerup = false;
    Color originalColour;
    private bool gameOver;

    //Controllers
    SoundController soundController;
    GameController gameController;
    Timer timer;
    CameraController cameraController;

    [Header("UI")]
    public GameObject inGamePanel;
    public GameObject gameOverPanel;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text winTimeText;

    // Start is called before the first frame update
    void Start()
    {
        baseSpeed = speed;
        rb = GetComponent<Rigidbody>();

        //Get the number of pickups in our scene
        pickupCount = GameObject.FindGameObjectsWithTag("Pick Up").Length;
        //Run the check pickips function
        SetCountText();
      
        //Turn on our In Game Panel
        inGamePanel.SetActive(true);
        //Turn off our Game Over Panel
        gameOverPanel.SetActive(false);

        gameController = FindObjectOfType<GameController>();
        timer = FindObjectOfType<Timer>();
        if (gameController.gameType == Gametype.SpeedRun)
            StartCoroutine(timer.StartCountdown());

        soundController = FindObjectOfType<SoundController>();

        //Enables reset point
        resetPoint = GameObject.Find("Reset Point");
        originalColour = GetComponent<Renderer>().material.color;

        cameraController = FindObjectOfType<CameraController>();

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameController.gameType == Gametype.SpeedRun && !timer.IsTiming())
            return;

        if (gameOver == true)
                return;

        if (resetting)
            return;

        if (grounded)
        {
            // Character Movement
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

            if (cameraController.cameraStyle == CameraStyle.Free)
            {
                //rotates the player to the direction of the camera
                transform.eulerAngles = Camera.main.transform.eulerAngles;
                //translates the input vectors into coordinates
                movement = transform.TransformDirection(movement);
            }

            rb.AddForce(movement * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pick Up")
        {
            Destroy(other.gameObject);
            //Decrement the pickup count
            pickupCount -= 1;
            //Run the check pickips function
            SetCountText();
            soundController.PlayPickupSound();
        }

        if(other.gameObject.CompareTag("Powerup"))
        {
            activePowerup = true;
            other.GetComponent<Powerup>().UsePowerup();
            soundController.PlayPowerupSound();
        }
    }

    void SetCountText()
    {
        //Display the ammount of pickups left in out scene
        scoreText.text = "Pickups Left: " + pickupCount;

        // Win condition 
        if (pickupCount == 0)
        {
            WinGame();

        }
    }

    void WinGame()
    {
        //Set the game over to true
        gameOver = true;
        //Turn on our Win Panel    
        gameOverPanel.SetActive(true);
        //Turn off our In Game Panel
        inGamePanel.SetActive(false);

        if (gameController.gameType == Gametype.SpeedRun)
            timer.StopTimer();

        soundController.PlayWinSound();

        //Set the Velocity of the rigidbody to zero
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            StartCoroutine(ResetPlayer());
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (activePowerup == false)
            {
                StartCoroutine(ResetPlayer());
            }
            else
            {
                //Logic for killing enemy
                soundController.PlayCollisionSound(collision.gameObject);
                Destroy(collision.gameObject);
                activePowerup = false;
            }
        }

        if (collision.gameObject.CompareTag("Door"))
        {
            if (activePowerup == true)
            {
                //Logic for breaking Objects
                soundController.PlayCollisionSound(collision.gameObject);
                Destroy(collision.gameObject);
                activePowerup = false;
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            soundController.PlayCollisionSound(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            grounded = false;
    }

    public IEnumerator ResetPlayer()
    {
        resetting = true;
        GetComponent<Renderer>().material.color = Color.white;
        rb.velocity = Vector3.zero;
        Vector3 startPos = transform.position;
        float resetSpeed = 2f;
        var i = 0.0f;
        var rate = 1.0f / resetSpeed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, resetPoint.transform.position, i);
            yield return null;
        }
        GetComponent<Renderer>().material.color = originalColour;
        resetting = false;

    }
}
