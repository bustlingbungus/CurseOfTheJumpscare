using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

public class Guy : Player
{
    /* ==========  FUNCTIONS  ========== */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        // get user input
        get_input(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKey(KeyCode.Space));
        base.Update();
    }
}
