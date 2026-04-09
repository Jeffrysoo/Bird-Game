using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject pipe;
    public float baseSpawnRate = 2f;
    private float timer = 0;
    public float heightOffSet = 10;
    public float spawnSpeedIncreasepoint = 0.05f;
    public float currentSpawnRate;
    public float minSpawnRate = 0.75f; //maximum spawn rate
    private LogicScript logic;

    public GameObject fruit;
    public float fruitSpawnChance = 0.3f; // 30% chance to spawn a fruit
    public float fruitOffsetX = 7f;
    public float randomFruitOffSetY;

    

    public float maxFruitUp = 1.5f;
    public float minFruitDown = -1.5f;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();
        SpawnPipe();
    }

    // Update is called once per frame
    void Update()
    {   
        currentSpawnRate = baseSpawnRate - (logic.playerScore * spawnSpeedIncreasepoint);

        // cannot exceed max spawn rate
        if (currentSpawnRate < minSpawnRate)
        {
            currentSpawnRate = minSpawnRate;
        }

        if (timer < currentSpawnRate)
        {
            timer += Time.deltaTime; 
        }
        else
        {
            SpawnPipe();
            timer = 0;
        }
        
    }

    public void SpawnPipe()
    {   
        float lowestPoint = transform.position.y - heightOffSet;
        float highestPoint = transform.position.y + heightOffSet;
        float spawnY = Random.Range(lowestPoint, highestPoint);

        Instantiate(pipe, new Vector3(transform.position.x, spawnY , 0) ,transform.rotation);

        randomFruitOffSetY = Random.Range(minFruitDown, maxFruitUp);

        if (fruit != null && Random.value < fruitSpawnChance)
        {
            Instantiate(fruit, new Vector3(transform.position.x + fruitOffsetX, spawnY + randomFruitOffSetY), transform.rotation);
        }


       
    }

  
}
