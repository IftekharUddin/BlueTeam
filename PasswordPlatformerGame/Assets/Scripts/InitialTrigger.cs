using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float width = 0;

        int numChildren = 0;
        float xPositions = 0;
        float yPositions = 0;
        foreach (Transform child in transform)
        {
            width += child.gameObject.GetComponent<Renderer>().bounds.size.x;
            xPositions += child.position.x;
            yPositions += child.position.y;
            numChildren++;
        }

        float xMiddle = xPositions / numChildren;
        float yMiddle = yPositions / numChildren;

        Vector3 right = new Vector3(xMiddle, yMiddle, 0) + (width / 2) * Vector3.right;

        PlatformGenerator.Instance.setRight(right);

        PlatformGenerator.Instance.generatePlatforms(4);
    }
}
