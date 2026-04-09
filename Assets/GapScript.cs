using JetBrains.Annotations;
using UnityEngine;

public class GapScript : MonoBehaviour


{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform topPipe;
    public Transform bottomPipe;
    public Transform middleGap;

    public float maxGap = 7.0f;
    public float minGap = 3.0f;

    public float pipeCenterOffset = 20f;
    void Start()
    {
        float gapSize = Random.Range(maxGap, minGap);



        if(topPipe!=null && bottomPipe!=null)
        {
            topPipe.localPosition = new Vector3 (topPipe.localPosition.x, (gapSize/2f) + pipeCenterOffset, 0);
            bottomPipe.localPosition = new Vector3(bottomPipe.localPosition.x, -(gapSize / 2f) - pipeCenterOffset, 0);
        }

        if(middleGap!=null)
        {
            middleGap.localScale = new Vector3(middleGap.localScale.x, gapSize, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
