using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This singleton holds the materials and fonts needed by other procedurally generated objects in one place so each 
/// individual object does not have to go get them.
/// </summary>
public class MaterialController : MonoBehaviour
{
    public int FEEDBACK_LAYER = 11;
    private static MaterialController _instance;
    // the material used by platforms
    public PhysicsMaterial2D platformMaterial;
    // the material used by the TextMesh 
    public Material textMaterial;
    // the font used by the TextMesh - can easily be changed by substituting the file in the Resources folder
    public Font textFont;

    /// <value> The singleton instance of this class which can be accessed by any other GameObject </value>
    public static MaterialController Instance
    {
        get
        {
            return _instance;
        }
    }

    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
