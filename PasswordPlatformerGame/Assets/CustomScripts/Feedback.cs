using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feedback : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < 8)
        {
            Destroy(this.gameObject);
        }
    }
}
