using UnityEngine;

public class PortalBottom : MonoBehaviour
{
    public GameObject portalTop;

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;
        other.transform.position = this.portalTop.transform.position;
    }
}
