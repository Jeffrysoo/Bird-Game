using UnityEngine;
using UnityEngine.SceneManagement; // Needed to restart the game when it dies
using System.IO;

public class QBrain : MonoBehaviour
{
    [Header("Grid & Vision Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float maxSeeDistanceX = 20f;
    public float maxSeeDistanceY = 15f;

    [Header("AI Learning Settings (Hyperparameters)")]
    public float learningRate = 0.1f;    // Alpha: How fast it overwrites old memories
    public float discountFactor = 0.9f;  // Gamma: How much it cares about the future
    public float explorationRate = 1.0f; // Epsilon: Starts at 100% random actions
    public float epsilonDecay = 0.99f;   // How fast it stops being random
    public float minExploration = 0.05f; // Keep 5% randomness so it never gets stuck

    [Header("Physics Settings")]
    public float flapForce = 5f;
    public float topDeadzone = 10f;     // The Y height where the bird dies going up
    public float bottomDeadzone = -10f; // The Y height where the bird dies falling

    public float brainSpeed = 0.1f; // The AI will think every 0.1 seconds
    private float decisionTimer = 0f;
    private static float currentExplorationRate = -1f; // STATIC so it remembers across deaths!

    private static float[,,] qTable;

    private static bool isInitialized = false;

    private Rigidbody2D rb;

    // Memory variables so the bird remembers what it JUST did
    private int[] previousState;
    private int previousAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (isInitialized == false)
        {
            qTable = new float[gridWidth, gridHeight, 2];
            InitializeBrain();

            // Set the static randomness the very first time!
            currentExplorationRate = explorationRate;

            isInitialized = true;
        }

        previousState = GetCurrentState();
    }

    // We use FixedUpdate because it locks exactly with the physics engine
    void FixedUpdate()
    {
        // Death Condition 2: Flying into the infinite sky or falling into the abyss
        if (transform.position.y > topDeadzone || transform.position.y < bottomDeadzone)
        {
            Die();
            return; // Stop reading the rest of the FixedUpdate code!
        }

        // Add time to our clock
        decisionTimer += Time.fixedDeltaTime;

        // ONLY think if 0.1 seconds have passed!
        if (decisionTimer >= brainSpeed)
        {
            decisionTimer = 0f; // Reset the clock

            int action = ChooseAction(previousState);

            if (action == 1)
            {
                rb.linearVelocity = Vector2.up * flapForce;
            }

            float reward = 1f;
            int[] newState = GetCurrentState();

            UpdateQTable(previousState, action, reward, newState);

            previousState = newState;
            previousAction = action;
        }
    }

    void Update()
    {
        // If you press the 'S' key on your keyboard, permanently save the brain!
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveBrain();
        }
    }

    // --- AI LOGIC METHODS ---

    int ChooseAction(int[] state)
    {
        // EXPLORE: Roll the dice. If random number is lower than Exploration Rate, do something random!
        if (Random.value < currentExplorationRate)
        {
            return Random.Range(0, 2);
        }

        // EXPLOIT: Look at the database. Which action has a higher score?
        float doNothingScore = qTable[state[0], state[1], 0];
        float flapScore = qTable[state[0], state[1], 1];

        if (flapScore > doNothingScore) return 1;
        else return 0;
    }

    void UpdateQTable(int[] oldState, int actionTaken, float reward, int[] newState)
    {
        // Look up the highest possible score we can get in the NEW state
        float maxFutureScore = Mathf.Max(qTable[newState[0], newState[1], 0], qTable[newState[0], newState[1], 1]);

        // Look up our OLD score
        float currentQValue = qTable[oldState[0], oldState[1], actionTaken];

        // The Magic Formula: Calculate the new, smarter score
        float newQValue = currentQValue + learningRate * (reward + discountFactor * maxFutureScore - currentQValue);

        // Save it to the database!
        qTable[oldState[0], oldState[1], actionTaken] = newQValue;
    }

    // --- DEATH AND PUNISHMENT ---

    // We created a helper method so we can kill the bird from anywhere!
    private void Die()
    {
        // MASSIVE PUNISHMENT! Tell the database that the last action was a terrible idea.
        UpdateQTable(previousState, previousAction, -1000f, previousState);

        // Decay the exploration rate so the next generation is slightly less random
        currentExplorationRate = Mathf.Max(minExploration, currentExplorationRate * epsilonDecay);

        // Restart the game instantly for the next generation
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Death Condition 1: Hitting a Pipe
        if (collision.gameObject.CompareTag("Pipe"))
        {
            Die(); // Run the punishment!
        }
    }

    // --- VISION SYSTEM (Same as before) ---
    void InitializeBrain()
    {
        // Try to load a saved brain first!
        if (File.Exists(GetSavePath()))
        {
            LoadBrain();

            // CRITICAL: If we loaded a master brain, we want it to stop acting random!
            explorationRate = minExploration;
        }
        else
        {
            // If no save file exists, create a brand new random brain
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    qTable[x, y, 0] = Random.Range(-0.1f, 0.1f); // Do Nothing
                    qTable[x, y, 1] = Random.Range(-0.1f, 0.1f); // Flap
                }
            }
            Debug.Log("Created a brand new baby brain.");
        }
    }
    // --- SAVE AND LOAD MEMORY ---

    // Unity provides a safe, permanent folder on your computer for game saves!
    private string GetSavePath()
    {
        return Application.persistentDataPath + "/SmartBirdBrain.txt";
    }

    public void SaveBrain()
    {
        // 1. Create a massive string to hold our database
        string brainData = "";

        // 2. Loop through every single grid square in the brain
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // 3. Write down the score for "Do Nothing" and "Flap", separated by a comma
                brainData += qTable[x, y, 0] + "," + qTable[x, y, 1] + "\n";
            }
        }

        // 4. Create the physical file on the computer!
        File.WriteAllText(GetSavePath(), brainData);
        Debug.Log("BRAIN SAVED SUCCESSFULLY TO: " + GetSavePath());
    }

    public void LoadBrain()
    {
        string path = GetSavePath();

        // Check if the file actually exists first
        if (File.Exists(path))
        {
            // Read all the lines of text back into the game
            string[] lines = File.ReadAllLines(path);
            int lineIndex = 0;

            // Rebuild the 3D Array from the text file
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    string[] scores = lines[lineIndex].Split(',');

                    qTable[x, y, 0] = float.Parse(scores[0]);
                    qTable[x, y, 1] = float.Parse(scores[1]);

                    lineIndex++;
                }
            }
            Debug.Log("BRAIN LOADED! The AI remembers everything.");
        }
        else
        {
            Debug.LogWarning("No saved brain found. Starting from scratch!");
        }
    }

    public int[] GetCurrentState()
    { // 1. Scan the screen for every pipe
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        GameObject closestPipe = null;
        float closestDistance = Mathf.Infinity;

        // 2. Find the pipe that is directly in front of us
        foreach (GameObject pipe in pipes)
        {
            float distX = pipe.transform.position.x - transform.position.x;

            // We only care about pipes that are IN FRONT of us (we use -2f so it doesn't go blind while inside the pipe)
            if (distX > -2f && distX < closestDistance)
            {
                closestDistance = distX;
                closestPipe = pipe;
            }
        }

        // If there are no pipes on screen yet, just return a safe default grid square
        if (closestPipe == null) return new int[] { gridWidth - 1, gridHeight / 2 };

        // 3. We found the pipe! Measure the exact continuous distance
        float deltaX = closestPipe.transform.position.x - transform.position.x;

        // Because of how you brilliantly wrote your spawner, the pipe's Y is exactly the middle of the gap!
        float deltaY = closestPipe.transform.position.y - transform.position.y;

        // 4. Crush those complex decimals down into our simple integer grid!
        int stateX = MapToGrid(deltaX, 0, maxSeeDistanceX, gridWidth);

        // Y can be positive (bird is below gap) or negative (bird is above gap)
        int stateY = MapToGrid(deltaY, -maxSeeDistanceY, maxSeeDistanceY, gridHeight);

        // Return the final grid coordinates! e.g., [3, 4]
        return new int[] { stateX, stateY };
    } // Simplified for space
    int MapToGrid(float value, float minBoundary, float maxBoundary, int gridSize)
    {
        float clampedValue = Mathf.Clamp(value, minBoundary, maxBoundary);
        float percentage = (clampedValue - minBoundary) / (maxBoundary - minBoundary);
        int index = Mathf.FloorToInt(percentage * gridSize);
        if (index >= gridSize) index = gridSize - 1; // Safety check
        return index;
    }
}