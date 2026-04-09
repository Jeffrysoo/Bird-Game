using UnityEngine;

public class MiddlePipeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int score = 1;
    public LogicScript logic;
    public AudioSource pointSound;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==3)
        {
           logic.AddScore(score);
            pointSound.Play();
        }
        
    }
}
