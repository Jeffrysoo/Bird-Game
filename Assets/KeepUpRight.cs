using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    void LateUpdate()
    {
        // Force the text to always face straight forward and perfectly level
        transform.rotation = Quaternion.identity;
    }
}