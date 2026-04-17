using UnityEngine;

public class MiddlePipeScript : MonoBehaviour
{
    public int score = 1;
    public LogicScript logic;
    public AudioSource pointSound;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the human passes through
        if (collision.gameObject.CompareTag("Player"))
        {
            logic.AddPlayerScore(1);

            if (pointSound != null)
            {
                pointSound.Play();
            }
        }
        // If the AI passes through
        else if (collision.gameObject.CompareTag("AiBird"))
        {
            logic.AddAIScore(1);

           
        }
    }
}