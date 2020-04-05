using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private bool hasCollided = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            PlatformGenerator.Instance.generatePlatforms();
        }
    }
}
