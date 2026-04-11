using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Header("Speed Settings")]
    public float scrollSpeed = 2f;

    [Header("Teleport Settings")]
    public float deadzone = -20f;     // The X position where the picture falls off screen
    public float teleportPos = 20f;   // The X position to teleport it back to

    void Update()
    {
        // 1. Move the background left exactly like your pipes!
        transform.position = transform.position + (Vector3.left * scrollSpeed) * Time.deltaTime;

        // 2. Check if it went off the screen
        if (transform.position.x <= deadzone)
        {
            // 3. Teleport it to the back of the line!
            transform.position = new Vector3(teleportPos, transform.position.y, transform.position.z);
        }
    }
}