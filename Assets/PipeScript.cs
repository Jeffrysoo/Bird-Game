using UnityEngine;

public class PipeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float deadzone = -1;
    public float pipeSpeed;

    private LogicScript logic;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();
        pipeSpeed = logic.currentMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position=transform.position + (Vector3.left * pipeSpeed)*  Time.deltaTime;
        if (transform.position.x < deadzone)
        {
            Debug.Log("Pipe Deleted"); 
            Destroy(gameObject);
        }
    }
}
