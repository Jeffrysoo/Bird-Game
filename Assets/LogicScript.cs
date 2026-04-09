using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;

    public GameObject backgroud1;
    public GameObject backgroud2;
    public GameObject backgroud3;
    public int scoreToChangeBackground2 = 10;
    public int scoreToChangeBackground3 = 20;

    public float baseSpeed = 5f;
    public float speedincrease = 0.2f;
    public float currentMoveSpeed;
    public float maxMoveSpeed = 15f;


    void Start()
    {
        // Set the starting speed as soon as the game begins!
        currentMoveSpeed = baseSpeed;
    }

    public void AddScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();

        currentMoveSpeed = baseSpeed + (playerScore * speedincrease);

        if(currentMoveSpeed > maxMoveSpeed)
        {
            currentMoveSpeed = maxMoveSpeed;
        }

        if (playerScore>=scoreToChangeBackground2)
        {
            backgroud1.SetActive(false);
            backgroud2.SetActive(true);
            backgroud3.SetActive(false);
        }

        if (playerScore >= scoreToChangeBackground3)
        {
            backgroud1.SetActive(false);
            backgroud2.SetActive(false);
            backgroud3.SetActive(true);
        }
    }

    public void restartGame()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
