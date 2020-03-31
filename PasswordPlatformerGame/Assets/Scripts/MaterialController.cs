using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public int FEEDBACK_LAYER = 11;
    private static MaterialController _instance;
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

    // Start is called before the first frame update
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
