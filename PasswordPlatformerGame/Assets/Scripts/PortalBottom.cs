using UnityEngine;

/// <summary>
/// A way for the game to catch the player and teleport them to a particular spot above the screen. 
/// </summary>
public class PortalBottom : MonoBehaviour
{
    /// <value> The paired top object to which to send the player </value>
    public GameObject portalTop;

    void OnCollisionEnter2D(Collision2D col)
    {
        // collision settings have been set up in Physics (Edit>Project Settings>Physics 2D)
        // such that only the Player collides with the portal
        GameObject other = col.gameObject;
        other.transform.position = this.portalTop.transform.position;
    }
}
