#if UNITY_EDITOR

using System;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.TextCore;

public class Monster : Player
{
    [SerializeField] private Texture pointer;
    private Rect pointer_rect;

    // unit vector pointing to the guy
    private Vector3 dir = Vector3.zero;

    // give the unit vector pointing to the guy
    public void set_displacement(Vector3 dist)
    {
        dir = dist.normalized;
    }

    // determine when to render or not
    private bool render_gui = true;

    public void set_GUI_rendering(bool rendering)
    {
        render_gui = rendering;
    }

    /* ==========  FUNCTIONS  ========== */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        set_axes(2);
        pointer_rect = new Rect((3*Screen.width/4)-100, (Screen.height/2)-400, 200,200);
    }

    // Update is called once per frame
    public override void Update()
    {
        pointer_rect = new Rect((3*Screen.width/4)-100, (Screen.height/2)-400, 200,200);
        get_input();
        base.Update();
    }

    // GUI rendering 
    void OnGUI()
    {
        if (render_gui)
        {
            // rotate the pointer based on where the monster is looking
            // relative to the displacement vector
            Vector3 a = Facing(), b = dir;
            a.y = b.y = 0f; a.Normalize(); b.Normalize();

            float angle = MathF.Acos(Vector3.Dot(a, b))*Mathf.Rad2Deg;
            Vector3 cross = Vector3.Cross(a, b);
            if (cross.y<0f) angle *= -1f;
            GUIUtility.RotateAroundPivot(angle, new Vector2(pointer_rect.x+100,pointer_rect.y+400));
            GUI.Label(pointer_rect, pointer);
        }
    }
}

#endif