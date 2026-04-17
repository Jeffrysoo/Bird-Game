using UnityEngine;

public class FruitScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int bonusMarks = 5;
    public float fruitSpeed;
    public float deadzone = -10;
    private LogicScript logic;
    public AudioClip fruitSoundClip;



    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();

        fruitSpeed = logic.currentMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * fruitSpeed * Time.deltaTime);

        if (transform.position.x < deadzone)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bird"))
        {
            // Give points, and destroy the fruit!
            logic.AddPlayerScore(bonusMarks);
            AudioSource.PlayClipAtPoint(fruitSoundClip, Camera.main.transform.position); //Play the sound at the camera's location
            Destroy(gameObject);
        }
    }
}
