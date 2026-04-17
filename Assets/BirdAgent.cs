using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BirdAgent : Agent
{
    [Header("Physics Settings")]
    public float flapForce = 5f;
    public float topDeadzone = 10f;
    public float bottomDeadzone = -10f;

    private Rigidbody2D rb;
    private Vector3 startPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Remember exactly where we spawned!
    }

    // 1. REPLACES SCENE RELOAD: Runs instantly every time the bird dies
    public override void OnEpisodeBegin()
    {
        // Teleport the bird back to the start and kill its momentum
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }

    // 2. REPLACES THE 10x10 GRID: Feed the exact decimals to the Neural Network
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation 1: Where is the bird currently? (Y position)
        sensor.AddObservation(transform.position.y);

        // Observation 2: How fast is the bird falling right now?
        sensor.AddObservation(rb.linearVelocity.y);

        // Find the closest pipe
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        GameObject closestPipe = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject pipe in pipes)
        {
            float distX = pipe.transform.position.x - transform.position.x;
            if (distX > -2f && distX < closestDistance)
            {
                closestDistance = distX;
                closestPipe = pipe;
            }
        }

        if (closestPipe != null)
        {
            // Observation 3 & 4: Exact distance to the pipe's X and Y
            sensor.AddObservation(closestPipe.transform.position.x - transform.position.x);
            sensor.AddObservation(closestPipe.transform.position.y - transform.position.y);
        }
        else
        {
            // If no pipes are on screen yet, just feed it zeros so the math doesn't break
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    // 3. REPLACES CHOOSE ACTION: The Python brain tells us what button it decided to press
    public override void OnActionReceived(ActionBuffers actions)
    {
        // We only have 1 branch of Discrete actions, which gives us a 0 (do nothing) or a 1 (flap)
        int flapChoice = actions.DiscreteActions[0];

        if (flapChoice == 1)
        {
            rb.linearVelocity = Vector2.up * flapForce;
        }

        // Give the AI a tiny micro-reward just for staying alive this frame!
        AddReward(0.01f);

        // Check if we flew out of bounds
        if (transform.position.y > topDeadzone || transform.position.y < bottomDeadzone)
        {
            Die();
        }
    }

    // 4. DEATH & PUNISHMENT
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pipe"))
        {
            Die();
        }
    }

    private void Die()
    {
        AddReward(-1.0f); // Massive punishment!
        EndEpisode();     // Instantly resets the bird using OnEpisodeBegin() without reloading the scene!
        gameObject.SetActive(false);
    }

    // 5. BONUS: THE "HUMAN TESTER"
    // ML-Agents has a built-in feature to let YOU play as the AI to make sure your code works before training!
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = 0; // Default is do nothing

        // If you press Space, force the AI to choose "1" (flap)
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActions[0] = 1;
        }
    }
}