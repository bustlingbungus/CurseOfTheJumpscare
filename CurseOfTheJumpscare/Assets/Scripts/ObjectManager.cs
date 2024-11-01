using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class ObjectManager : MonoBehaviour
{
    /* ==========  PLAYER INPUTS  ========== */

    // button guy presses to pick up candy
    private string pickUpCandy = "Trigger1";
    // button monster presses to jumpscare 
    private string jumpscareGuy = "Trigger2";
    // the key to press to restart the game
    [SerializeField] private InputAction restart;


    /* ==========  RENDERING PARAMETERS  ========== */
 
    // candy count rendering options
    [SerializeField] private Rect candyCountRect = new Rect(0,0,50,50);
    [SerializeField] private int candyCountFontSize = 50;
    [SerializeField] private Color candyCountColour = Color.white;
    // styles used for text rendering
    private GUIStyle candy_count_style = new GUIStyle(); 


    /* ==========  CONSTANT GAME VARIABLES  ========== */

    // the maximum distance the guy can be from a candy to pick it up
    [SerializeField] private float maxDistToCandy = 1.0f;
    // how directly the guy needs to be facing candy to pick it up
    // 1 = needs to look directly at it, -1 = no requirement
    [SerializeField] private float candyLookReq = 0.5f;

    // the amount of candy required to win 
    [SerializeField] private int candyWinAmt = 5;

    // the maximum distance the monster can jumpscare from
    [SerializeField] private float maxJumpscareDist = 7.0f;
    // the amount of jumpscares required for monster victory
    [SerializeField] private int jumpscareWinAmt = 2;

    // list of valid positions the guy and monster can be teleported to 
    [SerializeField] private List<List<UnityEngine.Vector3>> spawnLocations = new List<List<UnityEngine.Vector3>>
    {
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(178f, 2f, 136f), new UnityEngine.Vector3(0f  ,185f,0f  ) }, 
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(237f, 2f, 125f), new UnityEngine.Vector3(0f  ,106f,0f  ) },
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(111f, 2f, 182f), new UnityEngine.Vector3(0f  ,80f ,0f  ) },
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(134f, 2f, 104f), new UnityEngine.Vector3(0f  ,339f,0f  ) },
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(195f, 2f, 188f), new UnityEngine.Vector3(0f  ,235f,0f  ) },
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(85f , 2f, 56f ), new UnityEngine.Vector3(350f,5f  ,0f  ) },
        new List<UnityEngine.Vector3>{ new UnityEngine.Vector3(49f , 2f, 125f), new UnityEngine.Vector3(355f,112f,0f  ) },
    };


    /* ==========  GAME VARIABLES  ========== */

    // the amount of candy the player has picked up
    private int candy_picked_up = 0;
    // the number of times the monster has jumpscared the guy
    private int num_jumpscares = 0;
    // whether or not either player has won
    private bool game_is_over = false;


    /* ==========  GAME OBJECTS  ========== */

    // guy player
    [SerializeField] private Guy guy;
    // monster player
    [SerializeField] private Monster monster;
    // prefab for jumpscare
    [SerializeField] private Jumpscare jumpscarePrefab;
    // prefab for monster victory
    [SerializeField] private GameObject monsterWinPrefab;
    // prefab for guy victory
    [SerializeField] private GameObject guyWinPrefab;
    // prefab for candy
    [SerializeField] private GameObject candyPrefab;

    // list of all candy objects
    [SerializeField] private List<Candy> candies = new List<Candy>();
    // the candy the guy player is close to, tracked to avoid searching candies every frame
    private Candy close_candy = null;
    // list of gameobjects created at runtime
    private List<GameObject> created_objects = new List<GameObject>();

    // current jumpscare object
    private Jumpscare curr_jumpscare = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start a new game;
        reset_game();
        candy_count_style.fontSize = candyCountFontSize;
        candy_count_style.normal.textColor = candyCountColour;
    }

    // Update is called once per frame
    void Update()
    {
        candy_count_style.fontSize = candyCountFontSize;
        candy_count_style.normal.textColor = candyCountColour;
        guy_input();
        monster_input();

        // restart requested
        restart.performed += context => reset_game();

        // check win conditions
        if (!game_is_over)
        {
            // guy victory
            if (candy_picked_up >= candyWinAmt) {
                GameObject obj = Instantiate(guyWinPrefab).gameObject;
                created_objects.Add(obj);
                game_is_over = true;
                monster.set_GUI_rendering(false);
            }
            // monster victory
            else if (num_jumpscares >= jumpscareWinAmt) {
                GameObject obj = Instantiate(monsterWinPrefab).gameObject;
                created_objects.Add(obj);
                game_is_over = true;
                monster.set_GUI_rendering(false);
            }
        }

        // check to see if the current jumpscare needs to be removed
        if (curr_jumpscare != null) if (curr_jumpscare.Probe()) remove_jumpscare();

        // give monster direction to guy
        monster.set_displacement(guy.transform.position - monster.transform.position);
    }

    // GUI rendering 
    void OnGUI()
    {
        // render UI
        if (curr_jumpscare == null && !game_is_over) {
            GUI.Label(candyCountRect, candy_picked_up.ToString(), candy_count_style);
        }
    }

    // input actions
    void OnEnable()
    {
        restart.Enable();
    }

    void OnDisable()
    {
        restart.Disable();
    }


    /* ==========  HELPER FUNCTIONS  ========== */

    // sets both player's positions to a random point in the spawn_locations list
    private void respawn_players()
    {
        // get random indices for the spawn locations list
        System.Random rnd = new System.Random();
        int guy_idx = rnd.Next(spawnLocations.Count), monster_idx = rnd.Next(spawnLocations.Count);
        // ensure the indices are different 
        if (guy_idx == monster_idx) monster_idx = (monster_idx+1)%spawnLocations.Count;

        // set the players' locations to the chosen locations
        guy.transform.position = spawnLocations[guy_idx][0];
        monster.transform.position = spawnLocations[monster_idx][0];

        // set look directions
        guy.set_facing(spawnLocations[guy_idx][1]);
        monster.set_facing(spawnLocations[monster_idx][1]);
    }

    // updates the closest candy member if the guy is too far away
    // handles pickup input from guy player
    private void guy_input()
    {
        UnityEngine.Vector3 dist = UnityEngine.Vector3.zero;
        // if the guy is too far from close candy, stop tracking it
        if (close_candy != null) {
            dist = close_candy.transform.position - guy.transform.position;
            if (dist.magnitude > maxDistToCandy) close_candy = null;
        }

        // if the guy tries to pickup a candy
        if (Input.GetAxis(pickUpCandy)!=0f)
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
        if (UnityEngine.Vector3.Dot(dir, guy.Facing()) >= candyLookReq)
        {
            // interact with the candy. If true, it has been picked up
            if (close_candy.interact())
            {
                // remove candy from the game, and increment score
                candies.Remove(close_candy);
                close_candy.Eat();
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
            if (disp.magnitude <= maxDistToCandy) {
                close_candy = candy; 
                attempt_candy_pickup(disp.normalized);
                break;
            }
        }
    }

    private void monster_input()
    {
        if (Input.GetAxis(jumpscareGuy)!=0f)
        {
            UnityEngine.Vector3 dist = guy.transform.position - monster.transform.position;
            if (dist.magnitude <= maxJumpscareDist)
            {
                // valid if the monster is looking at the guy AND both players are facing the same direction
                // monster looking at guy: dot product of monster facing and displacement is positive
                // facing same direction: dot product of facing vectors positive
                if (UnityEngine.Vector3.Dot(monster.Facing(), dist) > 0.0f && 
                    UnityEngine.Vector3.Dot(monster.Facing(), guy.Facing()) > 0.5f) {
                    jumpscare();
                }
            }
        }
    }

    // spawns two jumpscare objects 
    private void jumpscare()
    {
        // create jumpscare object
        curr_jumpscare = Instantiate(jumpscarePrefab);
        // put both players in random locations
        respawn_players();
        monster.set_GUI_rendering(false);
        num_jumpscares++;
    }

    // removes a jumpscare
    private void remove_jumpscare()
    {
        if (!game_is_over) monster.set_GUI_rendering(true);
        Destroy(curr_jumpscare.gameObject);
        curr_jumpscare = null;
    }

    // resets the game
    private void reset_game()
    {
        // destroy all created objects
        for (int i=0,n=created_objects.Count; i<n; i++) Destroy(created_objects[i]);
        created_objects.Clear();

        // reset trackers
        candy_picked_up = num_jumpscares = 0;
        close_candy = null;
        game_is_over = false;

        // ensure all candies are enabled 
        for (int i=0,n=candies.Count; i<n; i++) candies[i].Vomit();

        // spawn players in random locations
        respawn_players();
        monster.set_GUI_rendering(true);
    }
}
