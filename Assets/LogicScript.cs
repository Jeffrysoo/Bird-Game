
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicScript : MonoBehaviour
{
    [Header("Scores & UI")]
    public int playerScore = 0;
    public int aiScore = 0;
    public Text playerScoreText; // Drag your human score UI here
    public Text aiScoreText;     // Drag a new UI text element here for the AI

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public GameObject playerWinScreen;
    public TextMeshProUGUI finalPlayerScoreText; // Slot for your Player score text
    public TextMeshProUGUI finalAIScoreText;     // Slot for your AI score text

    [Header("Backgrounds")]
    public GameObject backgroud1; // Keeping your exact spelling so it doesn't break your Unity links!
    public GameObject backgroud2;
    public GameObject backgroud3;
    public int scoreToChangeBackground2 = 10;
    public int scoreToChangeBackground3 = 20;

    [Header("Speed Mechanics")]
    public float baseSpeed = 5f;
    public float speedincrease = 0.2f;
    public float currentMoveSpeed;
    public float maxMoveSpeed = 15f;

    [Header("Game Modes")]
    public GameObject aiBirdObject; // We need to control the AI's physical body
    private bool isSinglePlayer = false;

    public bool isTrainingAI = true;

    void Start()
    {
        currentMoveSpeed = baseSpeed;

        // Read the save file from the Main Menu
        int selectedMode = PlayerPrefs.GetInt("GameMode", 0);

        if (selectedMode == 0)
        {
            // --- SINGLE PLAYER MODE ---
            isSinglePlayer = true;

            // Turn off the AI Bird
            if (aiBirdObject != null) aiBirdObject.SetActive(false);

            // Turn off the AI Score UI on the screen
            if (aiScoreText != null) aiScoreText.gameObject.SetActive(false);
        }
        else
        {
            // --- RACE AI MODE ---
            isSinglePlayer = false;
        }


    }

    public void AddPlayerScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        playerScoreText.text = playerScore.ToString();

        UpdateWorldEnvironment();
    }

    public void AddAIScore(int scoreToAdd)
    {   
        if(!isSinglePlayer)
        {
            aiScore += scoreToAdd;
            aiScoreText.text = aiScore.ToString();

            UpdateWorldEnvironment();
        }
        
    }

    // This new function calculates the speed and backgrounds based on whoever is WINNING.
    private void UpdateWorldEnvironment()
    {
        // Find out who has the highest score
        int highestScore = Mathf.Max(playerScore, aiScore);

        // Speed increases based on the leader
        currentMoveSpeed = baseSpeed + (highestScore * speedincrease);
        if (currentMoveSpeed > maxMoveSpeed)
        {
            currentMoveSpeed = maxMoveSpeed;
        }

        // Background changes based on the leader
        if (highestScore >= scoreToChangeBackground3)
        {
            backgroud1.SetActive(false);
            backgroud2.SetActive(false);
            backgroud3.SetActive(true);
        }
        else if (highestScore >= scoreToChangeBackground2)
        {
            backgroud1.SetActive(false);
            backgroud2.SetActive(true);
            backgroud3.SetActive(false);
        }
    }

    public void restartGame()
    {


        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        if (isTrainingAI == false)
        {

            // --- SINGLE PLAYER GAME OVER ---
            if (isSinglePlayer)
            {
                gameOverScreen.SetActive(true);
                playerWinScreen.SetActive(false);
            }

            else
            {
                // THE REFEREE LOGIC: Compare the scores!
                if (playerScore > aiScore)
                {
                    // The Player won! Show the Win Screen.
                    finalAIScoreText.text = aiScore.ToString();
                    finalPlayerScoreText.text = playerScore.ToString();
                    playerWinScreen.SetActive(true);




                }
                else
                {
                    // The AI won, or it was a tie. Show the normal Game Over Screen.
                    gameOverScreen.SetActive(true);


                }
            }

            
        }
    }


}

