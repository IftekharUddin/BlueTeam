using UnityEngine;

/// <summary>
/// A singleton class which serves as a generator for platforms. To perform this duty, it stores the current rightmost
/// point of generation and generates to the right of this. The current method of generation is to generate one password
/// platform on the upper half of the screen and one on the lower half, but this method can be easily changed due to this
/// class's modularity. 
/// </summary>
public class PlatformGenerator : MonoBehaviour
{
    private static PlatformGenerator _instance;

    // these are "magic numbers" corresponding to the index of each sprite in the array Resources.LoadAll returns
    private const int LEFT_PLATFORM_SPRITE = 24;
    private const int MIDDLE_PLATFORM_SPRITE = 25;
    private const int RIGHT_PLATFORM_SPRITE = 27;
    private const int LEFT_PASSWORD_SPRITE = 6;
    private const int MIDDLE_PASSWORD_SPRITE = 31;
    private const int RIGHT_PASSWORD_SPRITE = 7;

    // a "magic number" corresponding to the Physics layer on which the platforms and portals reside
    private const int PLATFORM_LAYER = 9;
    private const int PORTAL_LAYER = 10;
    // the interenal value used to store the current rightmost point of generation
    private Vector3 right;

    // an array of Sprite tiles from the resources folder
    private Sprite[] sprites;
    // the width of one sprite - computed at load time
    private float oneWidth;
    // the height of one sprite
    private float height;

    // the y value of the camera (should not change during the game)
    private float cameraY;
    // the value subtracted or added to the camera's y position to generate the "upper half" or "lower half" of the screen without moving too far
    private float cameraHeight;

    // the material used by the TextMesh 
    private Material textMaterial;
    // the font used by the TextMesh - can easily be changed by substituting the file in the Resources folder
    private Font textFont;


    /// <value> The singleton instance of this class which can be accessed by whoever wants to generate platforms. </value>
    public static PlatformGenerator Instance
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

        // the y position of the camera should not change throughout the game
        this.cameraY = Camera.main.transform.position.y;

        // from the docs: height of an orthographic camera is 2f*orthographicSize - here, instead though 
        // we want to add or subtract from the center but not get too high
        this.cameraHeight = Camera.main.orthographicSize * 0.6f;

        // load the spritesheet from the Resources folder of the project
        this.sprites = Resources.LoadAll<Sprite>("");

        // make a fake gameobject to know the width and height of one platform sprite (all the same)
        GameObject fake = new GameObject();
        fake.transform.localScale = new Vector3(8, 8, 1);
        SpriteRenderer fakeRenderer = fake.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        fakeRenderer.sprite = sprites[LEFT_PLATFORM_SPRITE];

        this.oneWidth = fakeRenderer.bounds.size.x;
        this.height = fakeRenderer.bounds.size.y;

        Destroy(fake);

        // load the text material from the Resources folder
        Material[] mats = Resources.LoadAll<Material>("");
        this.textMaterial = mats[0];

        // load the text font from the Resources folder
        Font[] fonts = Resources.LoadAll<Font>("");
        this.textFont = fonts[0];
    }

    public void setRight(Vector3 right)
    {
        this.right = right;
    }

    private GameObject makeNewPlatform(GameObject parent)
    {
        // abstracts out the commonalities in the various GameObjects which make up a platform
        GameObject gameObj = new GameObject();
        gameObj.layer = PLATFORM_LAYER;
        gameObj.transform.localScale = new Vector3(8, 8, 1);
        gameObj.transform.parent = parent.transform;
        return gameObj;
    }

    private Sprite currentSprite(int platform, int size, bool password)
    {
        // returns the Sprite corresponding to the correct place in the platform
        // password platforms use different sprites than the non-password platforms
        if (password)
        {
            if (platform == 0)
            {
                return sprites[LEFT_PASSWORD_SPRITE];
            }
            else if (platform == size - 1)
            {
                return sprites[RIGHT_PASSWORD_SPRITE];
            }
            else
            {
                return sprites[MIDDLE_PASSWORD_SPRITE];
            }
        }
        else
        {
            if (platform == 0)
            {
                return sprites[LEFT_PLATFORM_SPRITE];
            }
            else if (platform == size - 1)
            {
                return sprites[RIGHT_PLATFORM_SPRITE];
            }
            else
            {
                return sprites[MIDDLE_PLATFORM_SPRITE];
            }
        }
    }

    private (GameObject, Vector3) generatePlatform(int width, (bool, bool) passwordData, Vector3 start, (float, float) yBounds)
    {
        // width - the number of blocks which makes up this platform
        // passwordData - a tuple of (whether this is a password platform, whether this is a good password)
        // start - the rightmost point of the previously generated platform
        // yBounds - a tuple of (minimum y point, maximum y point) on which to generate this platform

        float platformY = Random.Range(yBounds.Item1, yBounds.Item2);

        float totalWidth = this.oneWidth * width;

        // separate anywhere between 1 and 3 player lengths
        float separation = Random.Range(1f, 3f);

        // x coordinate of the middle of the new platform
        float middlePlatform = separation + (width / 2) * this.oneWidth;

        // remember here we are subtracting or adding from/to the middle (the parent's position)
        // in cases of even width, the middle of the platform is between two platforms => all platforms have their anchor in their middle
        // in cases of odd width => middle of the platform is the center of the middle platform => don't need the extra correction
        float currXPosition = (width % 2 == 1) ? (-(width / 2) * this.oneWidth) : (-(width / 2) * this.oneWidth + (this.oneWidth / 2));

        // set up the parent object with a position of the center of the platform
        GameObject parent = new GameObject();
        parent.layer = PLATFORM_LAYER;
        BoxCollider2D boxCollider = parent.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        boxCollider.size = new Vector2(totalWidth, this.height);
        parent.transform.position = new Vector3(start.x + middlePlatform, platformY, start.z);

        // add password components
        if (passwordData.Item1)
        {
            if (passwordData.Item2)
            {
                parent.AddComponent(typeof(GoodPlatform));
            }
            else
            {
                parent.AddComponent(typeof(BadPlatform));
            }
        }
        else
        {
            parent.AddComponent(typeof(PlatformTrigger));
        }

        GameObject curr;
        SpriteRenderer currRenderer;
        for (int i = 0; i < width; i++)
        {
            Sprite currSprite = currentSprite(i, width, passwordData.Item1);

            curr = makeNewPlatform(parent);
            currRenderer = curr.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            currRenderer.sprite = currSprite;
            curr.transform.position = parent.transform.position + currXPosition * Vector3.right;
            currXPosition += oneWidth;
        }

        return (parent, new Vector3(start.x + separation + totalWidth, platformY, start.z));
    }

    private Vector3 generatePasswordPlatform(int width, bool goodPassword, Vector3 start, (float, float) yBounds)
    {
        string pass = (goodPassword) ? PasswordGeneration.Instance.GetGoodPassword() : PasswordGeneration.Instance.GetBadPassword();

        int platformWidth = Mathf.RoundToInt(Mathf.Ceil((float)pass.Length / 3.5f));

        (GameObject, Vector3) res = res = generatePlatform(platformWidth, (true, goodPassword), start, yBounds);

        // we have to add the object which holds the TextMesh to password platforms
        GameObject text = new GameObject();
        text.layer = PLATFORM_LAYER;
        text.transform.parent = res.Item1.transform;
        text.transform.position = res.Item1.transform.position;

        if (goodPassword)
        {
            GoodPlatform goodPlatform = res.Item1.GetComponent<GoodPlatform>();
            goodPlatform.passwordHolder = text;
        }
        else
        {
            BadPlatform badPlatform = res.Item1.GetComponent<BadPlatform>();
            badPlatform.passwordHolder = text;
        }

        MeshRenderer meshR = text.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = this.textMaterial;

        TextMesh textM = text.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = pass;
        textM.color = Color.white;
        textM.characterSize = 0.25f;
        textM.font = this.textFont;
        textM.anchor = TextAnchor.MiddleCenter;

        return res.Item2;
    }

    private Vector3 generateFinalPlatform(int width, Vector3 start, (float, float) yBounds)
    {
        return generatePlatform(width, (false, false), start, yBounds).Item2;
    }

    private void generatePortals(Vector3 start, Vector3 end)
    {
        // generates portals above and below the platforms 
        float actualCameraHeight = this.cameraHeight / 0.6f;

        // a top portal above the last lip of the starting platform
        GameObject portalTop = new GameObject();
        portalTop.layer = PORTAL_LAYER;
        portalTop.transform.position = new Vector3(start.x - 1.5f, this.cameraY + actualCameraHeight + 1f, 0);
        portalTop.transform.localScale = new Vector3(3f, 1f, 1f);

        SpriteRenderer renderer = portalTop.AddComponent<SpriteRenderer>() as SpriteRenderer;
        renderer.sprite = sprites[sprites.Length - 1];

        // a bottom portal which stretches from the end of the start platform to the end of the last platform
        GameObject portalBottom = new GameObject();
        portalBottom.layer = PORTAL_LAYER;
        portalBottom.transform.position = new Vector3((start.x + end.x) / 2, this.cameraY - actualCameraHeight - 1f, 0);
        // subtract here to prevent strange overlapping - this can be edited
        portalBottom.transform.localScale = new Vector3(Mathf.Abs(end.x - start.x) - 0.5f, 1f, 1f);

        renderer = portalBottom.AddComponent<SpriteRenderer>() as SpriteRenderer;
        renderer.sprite = sprites[sprites.Length - 1];

        portalBottom.AddComponent(typeof(BoxCollider2D));

        PortalBottom portalBottomScript = portalBottom.AddComponent(typeof(PortalBottom)) as PortalBottom;
        portalBottomScript.portalTop = portalTop;
    }

    public void generatePlatforms()
    {
        // we want one platform to go up, one to go down
        ((float, float), (float, float)) yBounds = (Random.value > 0.5f) ? ((this.cameraY - this.cameraHeight, this.cameraY), (this.cameraY, this.cameraY + this.cameraHeight)) : ((this.cameraY, this.cameraY + this.cameraHeight), (this.cameraY - this.cameraHeight, this.cameraY));
        // randomize the order of the good and bad password
        (bool, bool) goodBad = (Random.value > 0.5f) ? (true, false) : (false, true);

        Vector3 start = this.right;

        Vector3 vec = generatePasswordPlatform(Mathf.FloorToInt(Random.Range(3f, 6f)), goodBad.Item1, this.right, yBounds.Item1);
        vec = generatePasswordPlatform(Mathf.FloorToInt(Random.Range(3f, 6f)), goodBad.Item2, vec, yBounds.Item2);
        this.right = generateFinalPlatform(Mathf.FloorToInt(Random.Range(5f, 11f)), vec, (this.cameraY - this.cameraHeight, this.cameraY + this.cameraHeight));

        this.generatePortals(start, this.right);
    }

    public void generatePlatforms(int number)
    {
        // overloaded method allowing to generate multiple platforms - necessary at the start of the game to make sure
        // the player does not get to a point where they can see empty space
        for (int i = 0; i < number; i++)
        {
            this.generatePlatforms();
        }
    }
}
