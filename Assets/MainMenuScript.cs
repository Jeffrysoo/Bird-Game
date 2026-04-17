using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string gameSceneName = "SampleScene";
    public string menuSceneName = "MainMenu";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlaySinglePlayer()
    {
        // Save a "0" into the invisible save file. 0 = Single Player
        PlayerPrefs.SetInt("GameMode", 0);

        // Load the game!
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayAgainstAI()
    {
        // Save a "1" into the invisible save file. 1 = AI Race Mode
        PlayerPrefs.SetInt("GameMode", 1);

        // Load the game!
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }


}
