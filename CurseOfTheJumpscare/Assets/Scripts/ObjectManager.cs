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


    /* ==========  GAME OBJECTS  ========== */

    // guy player
    public Guy guy;
    // monster player
    public Monster monster;

    // list of all candy on the map
    public List<Candy> candies;
    // the candy the player is standing near to. keep track of it se that we don't 
    // have to search the entire candies list every frame
    private Candy close_candy = null;

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
        // check to see if the guy is still in range of close candy
        UnityEngine.Vector3 dist = UnityEngine.Vector3.zero;
        if (close_candy != null) {
            dist = close_candy.transform.position - guy.transform.position;
            // n0 longer in range of candy
            if (dist.magnitude > MAX_DIST_TO_CANDY) close_candy = null;
        }

        // if the guy is trying to pick up candy
        if (Input.GetMouseButton(pickUpCandy))
        {
            // if there is not currently candy being tracked, see if the player is standing close to a candy
            if (close_candy == null)
            {
                for (int i=0, n=candies.Count; i<n; i++)
                {
                    Candy candy = candies[i];
                    UnityEngine.Vector3 disp = candy.transform.position - guy.transform.position;
                    // the guy is standing close to the candy, update `close_candy` and stop searching
                    if (disp.magnitude <= MAX_DIST_TO_CANDY) {
                        close_candy = candy; break;
                    }
                }
            } // if the guy is already standing close to a candy, see if they are looking at it
            else 
            {
                UnityEngine.Vector3 lookDir = guy.Facing();
                // normalise dist to get the direction of the displacement as a unit vector
                dist.Normalize();
                // dot product of unit vectors: -1 <= x <= 1,
                // if the dot product is greater than the specified value, then the player is looking at the candy   
                if (UnityEngine.Vector3.Dot(dist, lookDir) >= CANDY_LOOK_REQ) close_candy.interact();
            }
        }
    }
}
