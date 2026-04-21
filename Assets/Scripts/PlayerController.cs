using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Singleton instance for easy access from other scripts (e.g., Coin)
    public static PlayerController Instance;

    void Awake()
    {
        Instance = this;
    }

    public  float   maxSpeed        = 5f;
    public  float   thrustForce     = 1f;
    public  float   pointsPerSecond = 10f;
    public  bool    deathEnabled    = true;

    public GameObject BoosterFlame;
    public GameObject GameBorder;
    public GameObject DeathEffect;

    public ParticleSystem thrustVFX;

    public AudioSource ThrustStart;
    public AudioSource ThrustLoop;

    public UIDocument ScoreUIDocument;
    public UIDocument RestartUIDocument;

    public System.Action OnDeath;

    private float   elapsedTime = 0f;
    private float   fpsTimer    = 0f;
    private int     score       = 0;
    private int     money       = 0;
    private int     currentHighScore;
    bool            isThrusting = false;
    private Label   scoreLabel;
    private Label   highScoreLabelText;
    private Label   highScoreLabel;
    private Label   moneyLabel;
    private Label   newBest;
    private Label   fpsLabel;
    private Button  restartButton;
    private ParticleSystem.EmissionModule emission;
    Camera cam;

    Rigidbody2D rb;

    // functions
    void UpdateScore()
    {
        // Update the elapsed time and calculate the score based on the points per second
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * pointsPerSecond);

        // Update the score display in the UI
        scoreLabel.text = $"{score}";
        
        //Debug.Log($"Elapsed Time: {score}");
    }
    void UpdatePlayerMovment()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            { 
                isThrusting = true;

                // SFX starting sound
                ThrustStart.loop = false;
                ThrustStart.Play();

                StopAllCoroutines();
                StartCoroutine(PlayThrustLoop());

                // VFX starting effect
                thrustVFX.Play();
                emission.rateOverTime = 100f;
            }

            // Get the mouse position in world space and calculate the direction from the player to the mouse position
            Vector3 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Rotate the player to face the direction of movement and apply a force in that direction
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            // Limit the player's speed to the maximum speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }

            // Activate the booster flame when the left mouse button is pressed
            BoosterFlame.SetActive(true);

            //Debug.Log("Left mouse button is pressed");
            //Debug.Log($"Left mouse button pressed at: {mousePos}");
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isThrusting = false;

            // Stopping SFX
            ThrustStart.Stop();
            ThrustLoop.Stop();

            // stopping VFX
            emission.rateOverTime = 0f;
            thrustVFX.Stop(false, ParticleSystemStopBehavior.StopEmitting);

            // Deactivate the booster flame when the left mouse button is released
            BoosterFlame.SetActive(false);
        }
    }
    public void UpdateMoney(int amount)
    {
        if (amount != 0)
        {
            money = PlayerPrefs.GetInt("Money", 0);
            //Debug.Log($"Current Money before update: {money}");
            money += amount;
            //Debug.Log($"Money updated by {amount}, new total: {money}");
            PlayerPrefs.SetInt("Money", money);
            PlayerPrefs.Save();
        }

        //Debug.Log($"Current Money: {money}");

        moneyLabel.text = $"{money} Coins";
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Die()
    {
        OnDeath?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // FOR DEBUG ONLY
        //PlayerPrefs.DeleteAll();

        rb = GetComponent<Rigidbody2D>();

        // Get references to UI elements
        scoreLabel          = ScoreUIDocument.rootVisualElement.Q<Label>("ScoreDisplay");
        highScoreLabelText  = RestartUIDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        highScoreLabel      = RestartUIDocument.rootVisualElement.Q<Label>("HighScore");
        moneyLabel          = ScoreUIDocument.rootVisualElement.Q<Label>("CoinDisplay");
        newBest             = RestartUIDocument.rootVisualElement.Q<Label>("NewBest");
        restartButton       = RestartUIDocument.rootVisualElement.Q<Button>("RestartButton");
        money = PlayerPrefs.GetInt("Money", 0);
        UpdateMoney(0); // Initialize money display

        // Hide the restart button and high score display at the start of the game
        restartButton.style.display         = DisplayStyle.None;
        highScoreLabelText.style.display    = DisplayStyle.None;
        highScoreLabel.style.display        = DisplayStyle.None;
        newBest.style.display               = DisplayStyle.None;

        // Get reference to the FPS label in the UI
        fpsLabel = ScoreUIDocument.rootVisualElement.Q<Label>("FPSDisplay");

        // Add a click event listener to the restart button to reload the scene when clicked
        restartButton.clicked += ReloadScene;

        // Initialize the thrust VFX emission rate to 0 at the start of the game
        emission = thrustVFX.emission;
        emission.rateOverTime = 0;
        thrustVFX.Stop();

        // Get the current high score from PlayerPrefs
        currentHighScore = PlayerPrefs.GetInt("HighScore", 0);

        // Cache the main camera reference
        cam = Camera.main;

        Application.targetFrameRate = -1;  // -1 = unlimited
        QualitySettings.vSyncCount = 0;    // vSync muss aus sonst wird targetFrameRate
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();


        fpsTimer += Time.deltaTime;
        if (fpsTimer >= 0.5f) // Update FPS every 0.5 seconds
        {
            fpsLabel.text = $"{Mathf.RoundToInt(1f / Time.deltaTime)} FPS";
            fpsTimer = 0f;
        }

            UpdatePlayerMovment();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (deathEnabled)
        {
            if (collision.gameObject.CompareTag("Coin"))
            {

            }
            else
            {
                // kill the player
                Instantiate(DeathEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
                Die();

                // Destroy the fame border
                Destroy(GameBorder);

                // Stop all SFX and VFX
                thrustVFX.Stop();
                ThrustStart.Stop();
                ThrustLoop.Stop();

                // Load high score from PlayerPrefs
                currentHighScore = PlayerPrefs.GetInt("HighScore", 0);

                // Check if the current score is higher than the stored high score and update it if necessary
                if (score > currentHighScore)
                {
                    // Saving high score to PlayerPrefs
                    PlayerPrefs.SetInt("HighScore", score);
                    PlayerPrefs.Save();
                    currentHighScore = PlayerPrefs.GetInt("HighScore", 0);

                    // Show "New Best" label if the player achieved a new high score
                    newBest.style.display = DisplayStyle.Flex;
                }

                Debug.Log($"Player fucking died to: {collision.gameObject.name}, final score: {score}, current high score: {currentHighScore}");

                // Show UI for restart and high score
                highScoreLabelText.style.display = DisplayStyle.Flex;
                highScoreLabel.style.display = DisplayStyle.Flex;
                highScoreLabel.text = $"{currentHighScore}";
                restartButton.style.display = DisplayStyle.Flex;
            }
        }
    }
    
    System.Collections.IEnumerator PlayThrustLoop()
    {
        // Wait for the starting sound to finish before playing the loop
        yield return new WaitForSeconds(ThrustStart.clip.length);

        if (isThrusting)
        {
            ThrustLoop.loop = true;
            ThrustLoop.Play();
        }
    }
}

