using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class ObjectManager : MonoBehaviour
{
    /* ==========  PLAYER INPUTS  ========== */

    // button guy presses to pick up candy
    private string pickUpCandy = "Trigger1";
    // button monster presses to jumpscare 
    private string jumpscareGuy = "Trigger2";
    // the key to press to restart the game
    [SerializeField] private KeyCode restart = KeyCode.Space;


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

    // list of valid positions the guy and monster can be teleported to 
    public List<UnityEngine.Vector3> spawn_locations = new List<UnityEngine.Vector3>
    {
        new UnityEngine.Vector3(178f, 2f, 136f),
        new UnityEngine.Vector3(237f, 2f, 125f),
        new UnityEngine.Vector3(111f, 2f, 182f),
        new UnityEngine.Vector3(134f, 2f, 104f),
        new UnityEngine.Vector3(195f, 2f, 188f),
        new UnityEngine.Vector3(85f , 2f, 56f ),
        new UnityEngine.Vector3(49f , 2f, 125f),
    };

    // list of locations candy will spawn at
    public List<UnityEngine.Vector3> candy_locations = new List<UnityEngine.Vector3>
    {
        new UnityEngine.Vector3(140f,1f  ,196f),
        new UnityEngine.Vector3(203f,1f  ,158f),
        new UnityEngine.Vector3(151f,1f  ,126f),
        new UnityEngine.Vector3(108f,1f  ,162f),
        new UnityEngine.Vector3(255f,1f  ,123f),
        new UnityEngine.Vector3(252f,1f  ,117f),
        new UnityEngine.Vector3(70f ,13f ,104f),
        new UnityEngine.Vector3(85f ,32f ,129f),
        new UnityEngine.Vector3(89f ,37f ,122f),
        new UnityEngine.Vector3(107f,37f ,106f),
        new UnityEngine.Vector3(110f,34f ,90f ),
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
    public Guy guy;
    // monster player
    public Monster monster;
    // prefab for jumpscare
    public Jumpscare jumpscarePrefab;
    // prefab for monster victory
    public GameObject monsterWinPrefab;
    // prefab for guy victory
    public GameObject guyWinPrefab;
    // prefab for candy
    public GameObject candyPrefab;

    // list of all candy objects
    private List<Candy> candies = new List<Candy>();
    // the candy the guy player is close to, tracked to avoid searching candies every frame
    private Candy close_candy = null;
    // list of gameobjects created at runtime
    private List<GameObject> created_objects = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start a new game;
        reset_game();
    }

    // Update is called once per frame
    void Update()
    {
        guy_input();
        monster_input();

        // restart requested
        if (Input.GetKey(restart)) reset_game();

        // check win conditions
        if (!game_is_over)
        {
            // guy victory
            if (candy_picked_up >= CANDY_WIN_AMT) {
                GameObject obj = Instantiate(guyWinPrefab).gameObject;
                created_objects.Add(obj);
                game_is_over = true;
            }
            // monster victory
            else if (num_jumpscares >= JUMPSCARE_WIN_AMT) {
                GameObject obj = Instantiate(monsterWinPrefab).gameObject;
                created_objects.Add(obj);
                game_is_over = true;
            }
        }
    }


    /* ==========  HELPER FUNCTIONS  ========== */

    // sets both player's positions to a random point in the spawn_locations list
    private void respawn_players()
    {
        // get random indices for the spawn locations list
        System.Random rnd = new System.Random();
        int guy_idx = rnd.Next(spawn_locations.Count), monster_idx = rnd.Next(spawn_locations.Count);
        // ensure the indices are different 
        if (guy_idx == monster_idx) monster_idx = (monster_idx+1)%spawn_locations.Count;

        // set the players' locations to the chosen locations
        guy.transform.position = spawn_locations[guy_idx];
        monster.transform.position = spawn_locations[monster_idx];
    }

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
        if (UnityEngine.Vector3.Dot(dir, guy.Facing()) >= CANDY_LOOK_REQ)
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
            if (disp.magnitude <= MAX_DIST_TO_CANDY) {
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

    // spawns two jumpscare objects 
    private void jumpscare()
    {
        // create jumpscare object
        Instantiate(jumpscarePrefab);
        // put both players in random locations
        respawn_players();
        num_jumpscares++;
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

        // destroy all exising candy 
        for (int i=0,n=candies.Count; i<n; i++) Destroy(candies[i].gameObject);
        candies.Clear();

        // create new candies
        for (int i=0, n=candy_locations.Count; i<n; i++)
        {
            // instantiate a new candy at the current location
            GameObject obj = Instantiate(candyPrefab, candy_locations[i], new UnityEngine.Quaternion());
            Candy candy = obj.GetComponent<Candy>();
            // add candy to candies list
            candies.Add(candy);
        }

        // spawn players in random locations
        respawn_players();
    }
}
