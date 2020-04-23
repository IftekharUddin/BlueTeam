using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a script given to the first "normal platform," so the platform generator can know where to start procedurally
/// generating platforms.
/// </summary>
public class InitialTrigger : MonoBehaviour
{
    void Start()
    {
        float width = 0;

        int numChildren = 0;
        float xPositions = 0;
        float yPositions = 0;
        // iterates over the children of this object (each individual block)
        foreach (Transform child in transform)
        {
            width += child.gameObject.GetComponent<Renderer>().bounds.size.x;
            xPositions += child.position.x;
            yPositions += child.position.y;
            numChildren++;
        }

        float xMiddle = xPositions / numChildren;
        float yMiddle = yPositions / numChildren;

        // the rightmost point of the platform
        Vector3 right = new Vector3(xMiddle, yMiddle, 0) + (width / 2) * Vector3.right;

        PlatformGenerator.Instance.setRight(right);

        // generate enough platforms so that the user will never see blank space beyond
        PlatformGenerator.Instance.generatePlatforms(4);
    }
}
