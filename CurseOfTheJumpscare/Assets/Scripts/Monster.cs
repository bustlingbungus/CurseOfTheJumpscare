using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

public class Monster : Player
{
    /* ==========  INPUT KEYS  ========== */

    public KeyCode LOOK_LEFT = KeyCode.LeftArrow, LOOK_RIGHT = KeyCode.RightArrow,
            LOOK_UP = KeyCode.UpArrow, LOOK_DOWN = KeyCode.DownArrow;

    /* ==========  FUNCTIONS  ========== */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        // use arrow keys to determine look direction 
        float horzMouse = (Input.GetKey(LOOK_RIGHT)?1f:0f) - (Input.GetKey(LOOK_LEFT)?1f:0f),
              vertMouse = (Input.GetKey(LOOK_UP)?1f:0f) - (Input.GetKey(LOOK_DOWN)?1f:0f);

        get_input(horzMouse, vertMouse);
        base.Update();
    }
}
