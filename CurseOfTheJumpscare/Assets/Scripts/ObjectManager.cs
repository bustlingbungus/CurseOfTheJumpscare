using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

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

    // the maximum distance the monster can jumpscare from
    public float MAX_JUMPSCARE_DIST = 7.0f;
    // the amount of jumpscares required for monster victory
    public int JUMPSCARE_WIN_AMT = 2;


    /* ==========  GAME VARIABLES  ========== */

    // the amount of candy the player has picked up
    private int candy_picked_up = 0;
    // the number of times the monster has jumpscared the guy
    private int num_jumpscares = 0;


    /* ==========  GAME OBJECTS  ========== */

    // guy player
    public Guy guy;
    // monster player
    public Monster monster;

    // list of all candy objects
    public List<Candy> candies;
    // the candy the guy player is close to, tracked to avoid searching candies every frame
    private Candy close_candy = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        guy_input();
        monster_input();
    }


    /* ==========  HELPER FUNCTIONS  ========== */

    // updates the closest candy member if the guy is too far away
    // handles pickup input from guy player
    private void guy_input()
    {
        UnityEngine.Vector3 dist = UnityEngine.Vector3.zero;
        // if the guy is too far from close candy, stop tracking it
        if (close_candy != null) {
            dist = close_candy.transform.position - guy.transform.position;
            if (dist.magnitude > MAX_DIST_TO_CANDY) close_candy = null;
        }

        // if the guy tries to pickup a candy
        if (Input.GetMouseButton(pickUpCandy))
        {
            // if there is not a candy currently being tracked, try to see if any are close
            if (close_candy == null) search_for_candy();
            // otherwise, see if the guy is looking at the candy
            else attempt_candy_pickup(dist.normalized);
        }
    }

    // checks if the guy is looking at close_candy. if they are, interact with it. if 
    // close_candy has been interacted with long enough, increment score and destroy the candy
    // \param dir the unit vector pointing from the guy to the candy
    private void attempt_candy_pickup(UnityEngine.Vector3 dir)
    {
        // dot product of unit vectors: -1 <= x <= 1
        // if the dot product is greater than the required value, the guy is looking 
        // sufficiently close to the candy
        if (UnityEngine.Vector3.Dot(dir, guy.Facing()) >= CANDY_LOOK_REQ)
        {
            // interact with the candy. If true, it has been picked up
            if (close_candy.interact())
            {
                // remove candy from the game, and increment score
                candies.Remove(close_candy);
                close_candy.Destroy();
                candy_picked_up++;
                close_candy = null;
            }
        }
    }

    // searches candies list to see if the player is standing close to any candy. 
    // if a potential candy is found, begin attempting to pick it up
    private void search_for_candy()
    {
        // iterate through all candies
        for (int i=0, n=candies.Count; i<n; i++)
        {
            Candy candy = candies[i];
            UnityEngine.Vector3 disp = candy.transform.position - guy.transform.position;
            // close candy found, attempt to pick it up and stop searching
            if (disp.magnitude <= MAX_DIST_TO_CANDY) {
                close_candy = candy; 
                attempt_candy_pickup(disp.normalized);
                break;
            }
        }
    }

    private void monster_input()
    {
        if (Input.GetMouseButton(jumpscareGuy))
        {
            UnityEngine.Vector3 dist = guy.transform.position - monster.transform.position;
            if (dist.magnitude <= MAX_JUMPSCARE_DIST)
            {
                // valid if the monster is looking at the guy AND both players are facing the same direction
                // monster looking at guy: dot product of monster facing and displacement is positive
                // facing same direction: dot product of facing vectors positive
                if (UnityEngine.Vector3.Dot(monster.Facing(), dist) > 0.0f && 
                    UnityEngine.Vector3.Dot(monster.Facing(), guy.Facing()) > 0.0f) {
                    jumpscare();
                }
            }
        }
    }

    // performs a jumpscare
    private void jumpscare()
    {
        Debug.Log("Jumpscare!\n");
        num_jumpscares++;
    }
}
