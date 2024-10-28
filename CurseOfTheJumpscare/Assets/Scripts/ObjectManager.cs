using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    /* ==========  PLAYER INPUTS  ========== */

    // button guy presses to pick up candy
    public int pickUpCandy = 0; // left click
    // button monster presses to jumpscare 
    public int jumpscareGuy = 1;


    /* ==========  CONSTANT GAME VARIABLES  ========== */

    // the maximum distance the guy can be from a candy to pick it up
    public float MAX_DIST_TO_CANDY = 1.0f;
    // how directly the guy needs to be facing candy to pick it up
    // 1 = needs to look directly at it, -1 = no requirement
    public float CANDY_LOOK_REQ = 0.5f;

    // the amount of candy required to win 
    public int CANDY_WIN_AMT = 5;


    /* ==========  GAME VARIABLES  ========== */

    // the amount of candy the player has picked up
    private int candy_picked_up = 0;


    /* ==========  GAME OBJECTS  ========== */

    // guy player
    public Guy guy;
    // monster player
    public Monster monster;

    public Candy candy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        guy_input();
    }


    /* ==========  HELPER FUNCTIONS  ========== */

    // updates the closest candy member if the guy is too far away
    // handles pickup input from guy player
    private void guy_input()
    {
        if (candy != null)
        {
            UnityEngine.Vector3 dist = candy.transform.position - guy.transform.position;
            if (dist.magnitude <= MAX_DIST_TO_CANDY && Input.GetMouseButton(pickUpCandy))
            {
                dist.Normalize();
                if (UnityEngine.Vector3.Dot(dist, guy.Facing()) >= CANDY_LOOK_REQ)
                {
                    if (candy.interact())
                    {
                        Destroy(candy);
                        candy = null;
                    }
                }
            }
        }
    }
}
