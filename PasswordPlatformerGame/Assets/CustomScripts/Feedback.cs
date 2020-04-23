using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script destroys the Feedback objects once they get below a certain y position.
/// </summary>
public class Feedback : MonoBehaviour
{
    // a magic number for the lower bound y position 
    private const int LOWER_BOUND = 8;

    void Update()
    {
        if (this.transform.position.y < LOWER_BOUND)
        {
            Destroy(this.gameObject);
        }
    }
}
