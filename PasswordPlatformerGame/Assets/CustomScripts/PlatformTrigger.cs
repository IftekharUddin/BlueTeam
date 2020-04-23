using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script which generates more platforms when the player collides with its platform.
/// </summary>
public class PlatformTrigger : MonoBehaviour
{
    private bool hasCollided = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            PlatformGenerator.Instance.generatePlatforms();
            this.hasCollided = true;
        }
    }
}
