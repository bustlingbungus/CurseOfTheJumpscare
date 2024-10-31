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
        set_axes(1);
    }

    // Update is called once per frame
    public override void Update()
    {
        get_input();
        base.Update();
    }
}
