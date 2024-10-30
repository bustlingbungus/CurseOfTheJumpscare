using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;
// trsts
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
        get_input(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        base.Update();
    }
}
