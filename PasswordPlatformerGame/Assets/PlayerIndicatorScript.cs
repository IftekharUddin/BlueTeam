using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public GameObject obj;
    public Vector3 screenpos = Camera.main.WorldToScreenPoint(obj.transform.position);
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //check if player is on screen - if not place the indicator at the proper y value and the player's x value

    if (screenpos.x > 0 && screenpos.x < Screen.width &&
        screenpos.y > 0 && screenpos.y < Screen.height) {
          //remove indicator
          GameObject.Find("Arrow").setActive(false);
        } else {
          var ind = GameObject.Find("Arrow");
          ind.setActive(true);
          ind.transform.position.y = Screen.height - 5;
          ind.transform.position.x = Player.position.x;
        }


    }
}
